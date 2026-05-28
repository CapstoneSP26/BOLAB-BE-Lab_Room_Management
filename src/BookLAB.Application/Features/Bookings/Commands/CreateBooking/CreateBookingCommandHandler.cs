using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Integration;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Policies;
using BookLAB.Application.Features.Bookings.Events;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BookLAB.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, CreateBookingResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPolicyEvaluator _policyEvaluator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;

        public CreateBookingCommandHandler(
            IUnitOfWork unitOfWork,
            IPolicyEvaluator policyEvaluator,
            ICurrentUserService currentUserService,
            IMediator mediator,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _policyEvaluator = policyEvaluator;
            _currentUserService = currentUserService;
            _mediator = mediator;
            _notificationService = notificationService;
        }

        public async Task<CreateBookingResponse> Handle(CreateBookingCommand request, CancellationToken ct)
        {
            string? warningMessage = null;

            // ==================== BƯỚC 1: VALIDATE ĐẦU VÀO CƠ BẢN ====================
            if (request.StartTime >= request.EndTime)
                throw new BusinessException("Thời gian bắt đầu phải trước thời gian kết thúc.");

            int totalWeeks = Math.Min(request.RecurringCount > 0 ? request.RecurringCount : 1, 4);
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            var startUtc = request.StartTime.ToUniversalTime();
            var endUtc = request.EndTime.ToUniversalTime();

            // Lấy thông tin loại mục đích để trích xuất trọng số PriorityLevel công khai
            var purposeType = await _unitOfWork.Repository<PurposeType>().Entities
                .FirstOrDefaultAsync(p => p.Id == request.PurposeTypeId, ct);
            if (purposeType == null) throw new NotFoundException("Mục đích đặt phòng không hợp lệ.");

            // Quy ước đặc quyền: Nếu ID mục đích = 3 (School Event của PĐT) -> Duyệt thẳng (Bypass Approval)
            bool isBypassApproval = purposeType.Id == 3;

            // ==================== BƯỚC 2: TRANSACTION DỮ LIỆU CHÍNH THỨC ====================
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var room = await _unitOfWork.Repository<LabRoom>().Entities
                    .Include(r => r.RoomPolicies)
                    .FirstOrDefaultAsync(r => r.Id == request.LabRoomId, ct);

                if (room == null || !room.IsActive)
                    throw new NotFoundException("Phòng không tồn tại hoặc không hoạt động.");

                // 2.1. CHECK CONFLICT CÁ NHÂN (CHỈ ÁP DỤNG CHO GIẢNG VIÊN / SINH VIÊN)
                if (!isBypassApproval)
                {
                    for (int i = 0; i < totalWeeks; i++)
                    {
                        var weekStart = startUtc.AddDays(i * 7);
                        var weekEnd = endUtc.AddDays(i * 7);

                        var hasScheduleConflict = await _unitOfWork.Repository<Schedule>().Entities
                            .AnyAsync(s => s.LecturerId == currentUserId && s.IsActive && !s.IsDeleted &&
                                           s.StartTime < weekEnd && s.EndTime > weekStart, ct);

                        var hasBookingConflict = await _unitOfWork.Repository<BookingRequest>().Entities
                            .AnyAsync(b => b.RequestedByUserId == currentUserId &&
                                           b.BookingRequestStatus == BookingRequestStatus.Pending &&
                                           b.Booking.StartTime < weekEnd && b.Booking.EndTime > weekStart, ct);

                        if (hasScheduleConflict || hasBookingConflict)
                            throw new BusinessException($"Bạn đã có lịch bận hoặc một yêu cầu khác đang chờ duyệt trùng vào tuần {i + 1} ({weekStart:dd/MM/yyyy}).");
                    }
                }

                // 2.2. THUẬT TOÁN SWEEPING LINE KIỂM TRA SỨC CHỨA PHÒNG (CAPACITY)
                var overlappingSchedules = await _unitOfWork.Repository<Schedule>().Entities
                    .Where(s => s.LabRoomId == request.LabRoomId && s.IsActive && !s.IsDeleted &&
                                s.StartTime < endUtc.AddDays((totalWeeks - 1) * 7) && s.EndTime > startUtc)
                    .Select(s => new { s.StartTime, s.EndTime, s.StudentCount })
                    .ToListAsync(ct);

                var sweepingEvents = new List<(DateTimeOffset Time, int Count)>();
                foreach (var s in overlappingSchedules)
                {
                    sweepingEvents.Add((s.StartTime, s.StudentCount));
                    sweepingEvents.Add((s.EndTime, -s.StudentCount));
                }

                var sortedSweepingEvents = sweepingEvents.OrderBy(e => e.Time).ThenBy(e => e.Count).ToList();
                int peakStudents = 0;
                int currentStudentsInRoom = 0;
                foreach (var ev in sortedSweepingEvents)
                {
                    currentStudentsInRoom += ev.Count;
                    if (currentStudentsInRoom > peakStudents) peakStudents = currentStudentsInRoom;
                }

                int projectedPeak = peakStudents + request.StudentCount;
                if (projectedPeak > room.Capacity)
                {
                    warningMessage = $"Cảnh báo: Tại thời điểm cao nhất, phòng {room.RoomName} sẽ có {projectedPeak}/{room.Capacity} sinh viên.";
                }

                Guid firstBookingId = Guid.Empty;
                var cancelledScheduleIds = new List<Guid>();
                var rejectedBookingIds = new List<Guid>();

                // ==================== BƯỚC 3: VÒNG LẶP KHỞI TẠO VÀ XỬ LÝ MA TRẬN ƯU TIÊN PHÂN RÃ XUNG ĐỘT ====================
                for (int i = 0; i < totalWeeks; i++)
                {
                    var bookingId = Guid.NewGuid();
                    if (i == 0) firstBookingId = bookingId;

                    var weekStart = startUtc.AddDays(i * 7);
                    var weekEnd = endUtc.AddDays(i * 7);

                    // Trạng thái động dựa trên phân quyền Bypass công tác điều phối
                    var targetBookingStatus = isBypassApproval ? BookingStatus.Approved : BookingStatus.PendingApproval;
                    var targetRequestStatus = isBypassApproval ? BookingRequestStatus.Approved : BookingRequestStatus.Pending;

                    var booking = new Booking
                    {
                        Id = bookingId,
                        LabRoomId = request.LabRoomId,
                        SlotTypeId = request.SlotTypeId > 0 ? request.SlotTypeId : null,
                        StartTime = weekStart,
                        EndTime = weekEnd,
                        Recur = totalWeeks,
                        BookingStatus = targetBookingStatus,
                        BookingType = request.BookingType,
                        PurposeTypeId = request.PurposeTypeId,
                        StudentCount = request.StudentCount,
                        Reason = request.Reason,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = currentUserId
                    };

                    var bookingRequest = new BookingRequest
                    {
                        Id = Guid.NewGuid(),
                        BookingId = bookingId,
                        RequestedByUserId = currentUserId,
                        BookingRequestStatus = targetRequestStatus,
                        ResponsedByUserId = isBypassApproval ? currentUserId : null,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = currentUserId
                    };

                    await _unitOfWork.Repository<Booking>().AddAsync(booking);
                    await _unitOfWork.Repository<BookingRequest>().AddAsync(bookingRequest);

                    // --------------------------------------------------------------------------------
                    // LUỒNG XỬ LÝ SẠCH PHÒNG CHỈ KÍCH HOẠT KHI PHÒNG ĐÀO TẠO THỰC THI (SCHOOL_EVENT)
                    // --------------------------------------------------------------------------------
                    if (isBypassApproval)
                    {
                        // A. Hạ bệ toàn bộ LỊCH ĐÃ XÁC NHẬN (Schedules) cấp thấp hơn đang hoạt động trùng giờ
                        var actualLowerSchedules = await _unitOfWork.Repository<Schedule>().Entities
                            .AsNoTracking()
                            .Where(s => s.LabRoomId == request.LabRoomId && s.IsActive && !s.IsDeleted &&
                                        s.StartTime < endUtc && s.EndTime > startUtc &&
                                        (int)s.SchedulePriority < purposeType.PriorityLevel)
                            .ToListAsync(ct);

                        foreach (var oldSchedule in actualLowerSchedules)
                        {
                            oldSchedule.IsActive = false;
                            oldSchedule.ScheduleStatus = ScheduleStatus.Cancelled;
                            oldSchedule.IsDeleted = true;
                            oldSchedule.AutoCancelledByBookingId = bookingId; // GHI VẾT TÍCH KHÔI PHỤC OAN
                            _unitOfWork.Repository<Schedule>().Update(oldSchedule);
                            cancelledScheduleIds.Add(oldSchedule.Id);
                        }

                        // B. Triệt hạ toàn bộ HÀNG ĐỢI ĐANG CHỜ (Booking Requests) trùng giờ có priority thấp hơn
                        var actualLowerPendingBookings = await _unitOfWork.Repository<Booking>().Entities
                            .AsNoTracking()
                            .Where(b => b.LabRoomId == request.LabRoomId && b.Id != bookingId &&
                                        b.BookingStatus == BookingStatus.PendingApproval &&
                                        b.StartTime < booking.EndTime && b.EndTime > booking.StartTime &&
                                        b.PurposeTypeId < purposeType.PriorityLevel)
                            .ToListAsync(ct);

                        foreach (var lowBooking in actualLowerPendingBookings)
                        {
                            lowBooking.BookingStatus = BookingStatus.Rejected;
                            lowBooking.AutoRejectedByBookingId = bookingId; // GHI VẾT TÍCH GỬI MAIL THÔNG BÁO LẠI
                            _unitOfWork.Repository<Booking>().Update(lowBooking);
                            rejectedBookingIds.Add(lowBooking.Id);

                            var lowBookingReq = await _unitOfWork.Repository<BookingRequest>().Entities
                                .FirstOrDefaultAsync(x => x.BookingId == lowBooking.Id, ct);
                            if (lowBookingReq != null)
                            {
                                lowBookingReq.BookingRequestStatus = BookingRequestStatus.Rejected;
                                _unitOfWork.Repository<BookingRequest>().Update(lowBookingReq);
                            }
                        }

                        // C. BIẾN YÊU CẦU NÀY THÀNH LỊCH HOẠT ĐỘNG CHÍNH THỨC (SCHEDULE) NGAY LẬP TỨC
                        var newSchedule = new Schedule
                        {
                            Id = Guid.NewGuid(),
                            LabRoomId = request.LabRoomId,
                            LecturerId = currentUserId,
                            StartTime = weekStart,
                            EndTime = weekEnd,
                            ScheduleType = ScheduleType.Event,
                            SchedulePriority = SchedulePriority.SCHOOL_EVENT, // Đóng dấu cấp 3 cao nhất toàn trường
                            ScheduleStatus = ScheduleStatus.Active,
                            CreatedAt = DateTimeOffset.UtcNow,
                            CreatedBy = booking.CreatedBy,
                            IsActive = true,
                            IsDeleted = false,
                            StudentCount = request.StudentCount
                        };
                        await _unitOfWork.Repository<Schedule>().AddAsync(newSchedule);
                    }
                }

                // 2.4. ĐÁNH GIÁ CHÍNH SÁCH HẠN MỨC (Chỉ áp dụng với luồng đặt phòng cần phê duyệt thông thường)
                if (!isBypassApproval)
                {
                    var activePolicies = room.RoomPolicies.Where(p => p.IsActive).ToList();
                    await _policyEvaluator.EvaluateAsync(request, activePolicies);
                }

                // ==================== BƯỚC 4: THIẾT LẬP THÔNG BÁO VÀ HOÀN TẤT TRANSACTION ====================
                var metadata = JsonSerializer.Serialize(new { bookingId = firstBookingId, totalWeeks = totalWeeks, isRecurring = totalWeeks > 1 });
                await _unitOfWork.Repository<Notification>().AddAsync(new Notification
                {
                    UserId = currentUserId,
                    Title = isBypassApproval ? "Lịch sự kiện đặc biệt trường thiết lập trực tiếp" : "Đặt lịch thành công",
                    Message = isBypassApproval
                        ? $"Sự kiện trường độc quyền tại phòng {room.RoomName} đã được kích hoạt trực tiếp."
                        : $"Yêu cầu đặt phòng {room.RoomName} của bạn đã được gửi lên hàng đợi phê duyệt thành công.",
                    Type = isBypassApproval ? "ExclusiveBookingCreated" : "BookingCreated",
                    Metadata = JsonDocument.Parse(metadata).RootElement.Clone(),
                    IsRead = false,
                    IsGlobal = false,
                    CreatedAt = DateTimeOffset.UtcNow
                });

                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync();

                // ==================== BƯỚC 5: PHÁT PHÁT REALTIME NOTIFICATIONS & EVENTS BÊN NGOÀI ====================
                if (currentUserId != Guid.Empty)
                {
                    await _notificationService.NotifyBookingChangedAsync(currentUserId, new
                    {
                        action = isBypassApproval ? "approved" : "created",
                        bookingId = firstBookingId,
                        roomId = request.LabRoomId,
                        occurredAt = DateTimeOffset.UtcNow
                    }, ct);
                }

                // Nếu có lịch bị đá bay, phát tín hiệu SignalR đồng bộ trạng thái trống/bận Calendar toàn hệ thống ngay lập tức
                if (isBypassApproval)
                {
                    var statusChangedPayload = new { labRoomId = room.Id, startTime = request.StartTime, endTime = request.EndTime };
                    await _notificationService.NotifyScheduleStatusChangedAsync(statusChangedPayload, ct);

                    // Trigger Event phụ hỗ trợ gửi email báo động hủy lịch oan cho Giảng viên/Sinh viên bị đè
                    // allowedCreateSchedule = false
                    await _mediator.Publish(new BookingApprovedEvent(firstBookingId, currentUserId, rejectedBookingIds, cancelledScheduleIds, false), ct);
                }
                else
                {
                    var newBookingPayload = new { publisherId = currentUserId, labRoomId = room.Id, startTime = request.StartTime, endTime = request.EndTime };
                    await _notificationService.NotifyNewBookingAsync(newBookingPayload, ct);
                    await _mediator.Publish(new BookingCreatedEvent(firstBookingId), ct);
                }

                return new CreateBookingResponse
                {
                    BookingId = firstBookingId,
                    WarningMessage = warningMessage
                };
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}