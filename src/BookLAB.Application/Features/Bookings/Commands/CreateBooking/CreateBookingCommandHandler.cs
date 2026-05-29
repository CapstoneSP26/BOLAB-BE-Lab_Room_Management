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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

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

            var currentUserId = _currentUserService.UserId ?? Guid.Empty;
            var startUtc = request.StartTime.ToUniversalTime();
            var endUtc = request.EndTime.ToUniversalTime();

            // Trích xuất thông tin loại mục đích để lấy trọng số PriorityLevel
            var purposeType = await _unitOfWork.Repository<PurposeType>().Entities
                .FirstOrDefaultAsync(p => p.Id == request.PurposeTypeId, ct);
            if (purposeType == null) throw new NotFoundException("Mục đích đặt phòng không hợp lệ.");

            // Quy ước đặc quyền: Loại mục đích ID = 3 (School Event của PĐT) -> Tự động duyệt thẳng
            bool isBypassApproval = purposeType.Id == 3;

            // 🚨 CHẶN TỐI CAO: Lịch cấp độ độc quyền (Priority >= 2 - Academic & School Event) CẤM ĐẶT ĐỊNH KỲ
            if (purposeType.PriorityLevel >= 2 && request.RecurringCount > 1)
            {
                throw new BusinessException($"Mục đích '{purposeType.PurposeName}' mang tính chất đặc biệt/độc quyền, hệ thống chỉ cho phép đăng ký sử dụng đơn lẻ cho từng buổi riêng biệt (Tối đa 1 tuần).");
            }

            // Phân bổ số tuần hoạt động: Lịch thường tối đa 4 tuần, lịch độc quyền cố định duy nhất 1 tuần
            int totalWeeks = purposeType.PriorityLevel >= 2 ? 1 : Math.Min(request.RecurringCount > 0 ? request.RecurringCount : 1, 4);

            // ==================== BƯỚC 2: TRANSACTION DỮ LIỆU CHÍNH THỨC ====================
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var room = await _unitOfWork.Repository<LabRoom>().Entities
                    .Include(r => r.RoomPolicies)
                    .FirstOrDefaultAsync(r => r.Id == request.LabRoomId, ct);

                if (room == null || !room.IsActive)
                    throw new NotFoundException("Phòng máy không tồn tại hoặc đã tạm dừng hoạt động.");

                // 2.1. ĐÁNH CHẶN MA TRẬN ĐỘC QUYỀN TRÊN CÁC SCHEDULES ĐÃ ĐƯỢC DUYỆT TRONG DB
                var allOverlappingSchedules = await _unitOfWork.Repository<Schedule>().Entities
                    .Where(s => s.LabRoomId == request.LabRoomId && s.IsActive && !s.IsDeleted &&
                                s.StartTime < endUtc.AddDays((totalWeeks - 1) * 7) && s.EndTime > startUtc)
                    .ToListAsync(ct);

                // Vòng lặp quét kiểm tra va chạm cấu trúc độc quyền theo từng tuần đơn lẻ
                for (int i = 0; i < totalWeeks; i++)
                {
                    var weekStart = startUtc.AddDays(i * 7);
                    var weekEnd = endUtc.AddDays(i * 7);

                    var weekSchedules = allOverlappingSchedules
                        .Where(s => s.StartTime < weekEnd && s.EndTime > weekStart)
                        .ToList();

                    // KỊCH BẢN A: Phòng đã có lịch SCHOOL_EVENT (Mức 3) -> KHÓA CỨNG PHÒNG TUYỆT ĐỐI
                    bool hasSchoolEvent = weekSchedules.Any(s => s.SchedulePriority == SchedulePriority.SCHOOL_EVENT);
                    if (hasSchoolEvent)
                    {
                        throw new BusinessException($"Không thể đặt lịch. Tuần {i + 1} ({weekStart:dd/MM/yyyy}) phòng máy này đã bị khóa cứng bởi Sự kiện/Lịch thi độc quyền của Nhà trường.");
                    }

                    // KỊCH BẢN B: Yêu cầu là ACADEMIC/NORMAL nhưng đụng lịch ACADEMIC (Mức 2) đã duyệt sẵn
                    bool hasAcademicEvent = weekSchedules.Any(s => s.SchedulePriority == SchedulePriority.ACADEMIC);
                    if (hasAcademicEvent && !isBypassApproval)
                    {
                        throw new BusinessException($"Không thể tạo lịch. Tuần {i + 1} ({weekStart:dd/MM/yyyy}) phòng đã có lịch học chính khóa/dạy bù chính thức của Giảng viên khác.");
                    }
                }

                // 2.2. CHECK XUNG ĐỘT CÁ NHÂN (CHỈ ÁP DỤNG CHO LUỒNG ĐẶT LỊCH THƯỜNG)
                if (!isBypassApproval)
                {
                    for (int i = 0; i < totalWeeks; i++)
                    {
                        var weekStart = startUtc.AddDays(i * 7);
                        var weekEnd = endUtc.AddDays(i * 7);

                        bool hasScheduleConflict = await _unitOfWork.Repository<Schedule>().Entities
                            .AnyAsync(s => s.LecturerId == currentUserId && s.IsActive && !s.IsDeleted &&
                                           s.StartTime < weekEnd && s.EndTime > weekStart, ct);

                        bool hasBookingConflict = await _unitOfWork.Repository<BookingRequest>().Entities
                            .AnyAsync(b => b.RequestedByUserId == currentUserId &&
                                           b.BookingRequestStatus == BookingRequestStatus.Pending &&
                                           b.Booking.StartTime < weekEnd && b.Booking.EndTime > weekStart, ct);

                        if (hasScheduleConflict || hasBookingConflict)
                            throw new BusinessException($"Bạn đã có lịch bận cá nhân hoặc một yêu cầu khác đang chờ duyệt trùng vào tuần {i + 1} ({weekStart:dd/MM/yyyy}).");
                    }
                }

                // 2.3. THUẬT TOÁN SWEEPING LINE KIỂM TRA SỨC CHỨA PHÒNG (CAPACITY)
                var sweepingEvents = new List<(DateTimeOffset Time, int Count)>();
                foreach (var s in allOverlappingSchedules)
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
                    warningMessage = $"Cảnh báo: Tại thời điểm đông sinh viên nhất, phòng {room.RoomName} sẽ vượt tải công suất cấu hình ({projectedPeak}/{room.Capacity} chỗ).";
                }

                Guid firstBookingId = Guid.Empty;
                var cancelledScheduleIds = new List<Guid>();
                var rejectedBookingIds = new List<Guid>();

                // ==================== BƯỚC 3: VÒNG LẶP KHỞI TẠO VÀ PHÂN RÃ XUNG ĐỘT ƯU TIÊN ====================
                for (int i = 0; i < totalWeeks; i++)
                {
                    var bookingId = Guid.NewGuid();
                    if (i == 0) firstBookingId = bookingId;

                    var weekStart = startUtc.AddDays(i * 7);
                    var weekEnd = endUtc.AddDays(i * 7);

                    var targetBookingStatus = isBypassApproval ? BookingStatus.Approved : BookingStatus.PendingApproval;
                    var targetRequestStatus = isBypassApproval ? BookingRequestStatus.Approved : BookingRequestStatus.Pending;

                    var booking = new Booking
                    {
                        Id = bookingId,
                        LabRoomId = request.LabRoomId,
                        SlotTypeId = request.SlotTypeId > 0 ? request.SlotTypeId : null,
                        StartTime = weekStart,
                        EndTime = weekEnd,
                        Recur = totalWeeks, // Đối với Academic/School Event, biến này luôn bằng 1
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
                    // LUỒNG XỬ LÝ SẠCH PHÒNG (SWEEPING) CHỈ KÍCH HOẠT KHI PĐT IMPORT (SCHOOL_EVENT)
                    // --------------------------------------------------------------------------------
                    if (isBypassApproval)
                    {
                        // A. Hạ bệ toàn bộ LỊCH ĐÃ XÁC NHẬN (Schedules) cấp thấp hơn (Mức 2, 1) trùng giờ
                        var actualLowerSchedules = allOverlappingSchedules
                            .Where(s => s.StartTime < weekEnd && s.EndTime > weekStart &&
                                        (int)s.SchedulePriority < purposeType.PriorityLevel)
                            .ToList();

                        foreach (var oldSchedule in actualLowerSchedules)
                        {
                            oldSchedule.IsActive = false;
                            oldSchedule.ScheduleStatus = ScheduleStatus.Cancelled;
                            oldSchedule.IsDeleted = true;
                            oldSchedule.AutoCancelledByBookingId = bookingId; // Lưu vết để rollback nếu cần
                            _unitOfWork.Repository<Schedule>().Update(oldSchedule);
                            cancelledScheduleIds.Add(oldSchedule.Id);
                        }

                        // B. Triệt hạ toàn bộ HÀNG ĐỢI ĐANG CHỜ (Booking Requests) trùng giờ có priority thấp hơn
                        var actualLowerPendingBookings = await _unitOfWork.Repository<Booking>().Entities
                            .Include(b => b.PurposeType)
                            .Where(b => b.LabRoomId == request.LabRoomId && b.Id != bookingId &&
                                        b.BookingStatus == BookingStatus.PendingApproval &&
                                        b.StartTime < weekEnd && b.EndTime > weekStart &&
                                        b.PurposeType.PriorityLevel < purposeType.PriorityLevel)
                            .ToListAsync(ct);

                        foreach (var lowBooking in actualLowerPendingBookings)
                        {
                            lowBooking.BookingStatus = BookingStatus.Rejected;
                            lowBooking.AutoRejectedByBookingId = bookingId; // Đóng dấu bắn thông báo
                            lowBooking.PurposeType = null;
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

                        // C. BIẾN YÊU CẦU NÀY THÀNH LỊCH HOẠT ĐỘNG CHÍNH THỨC (SCHEDULE) TRÊN LƯỚI NGAY LẬP TỨC
                        var newSchedule = new Schedule
                        {
                            Id = Guid.NewGuid(),
                            LabRoomId = request.LabRoomId,
                            LecturerId = currentUserId,
                            StartTime = weekStart,
                            EndTime = weekEnd,
                            ScheduleType = ScheduleType.Event,
                            SchedulePriority = SchedulePriority.SCHOOL_EVENT, // Gán mức độ ưu tiên 3 cao nhất
                            ScheduleStatus = ScheduleStatus.Active,
                            CreatedAt = DateTimeOffset.UtcNow,
                            CreatedBy = currentUserId,
                            IsActive = true,
                            IsDeleted = false,
                            StudentCount = request.StudentCount
                        };
                        await _unitOfWork.Repository<Schedule>().AddAsync(newSchedule);
                    }
                }

                // 2.4. ĐÁNH GIÁ CHÍNH SÁCH HẠN MỨC QUY ĐỊNH (Chỉ áp dụng với luồng thường cần phê duyệt)
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
                        ? $"Sự kiện trường độc quyền tại phòng {room.RoomName} đã được kích hoạt trực tiếp từ hệ thống điều phối."
                        : $"Yêu cầu đặt phòng {room.RoomName} của bạn đã được gửi lên hàng đợi phê duyệt thành công.",
                    Type = isBypassApproval ? "ExclusiveBookingCreated" : "BookingCreated",
                    Metadata = JsonDocument.Parse(metadata).RootElement.Clone(),
                    IsRead = false,
                    IsGlobal = false,
                    CreatedAt = DateTimeOffset.UtcNow
                });

                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync();

                // ==================== BƯỚC 5: PHÁT TÍN HIỆU REALTIME NOTIFICATIONS & INTERNALS ====================
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

                if (isBypassApproval)
                {
                    var statusChangedPayload = new { labRoomId = room.Id, startTime = request.StartTime, endTime = request.EndTime };
                    await _notificationService.NotifyScheduleStatusChangedAsync(statusChangedPayload, ct);

                    // Kích hoạt MediatR Event để chạy Worker ngầm xử lý gửi mail thông báo cho các giảng viên bị đè lịch
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