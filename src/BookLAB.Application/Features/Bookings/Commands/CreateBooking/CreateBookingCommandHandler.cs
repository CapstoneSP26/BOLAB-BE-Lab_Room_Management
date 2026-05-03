using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Integration;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
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
            // 1. Khai báo & Validate cơ bản
            if (request.StartTime >= request.EndTime)
                throw new BusinessException("Thời gian bắt đầu phải trước thời gian kết thúc.");

            // Giới hạn cứng tối đa 4 tuần theo yêu cầu của bạn
            int totalWeeks = Math.Min(request.RecurringCount > 0 ? request.RecurringCount : 1, 4);
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var room = await _unitOfWork.Repository<LabRoom>().Entities
                    .Include(r => r.RoomPolicies)
                    .FirstOrDefaultAsync(r => r.Id == request.LabRoomId, ct);

                if (room == null || !room.IsActive)
                    throw new NotFoundException("Phòng không tồn tại hoặc không hoạt động.");

                var startUtc = request.StartTime.ToUniversalTime();
                var endUtc = request.EndTime.ToUniversalTime();

                var overlappingSchedules = await _unitOfWork.Repository<Schedule>().Entities
                    .Where(s => s.LabRoomId == request.LabRoomId && s.IsActive && !s.IsDeleted &&
                                s.StartTime < endUtc && s.EndTime > startUtc)
                    .Select(s => new { s.StartTime, s.EndTime, s.StudentCount })
                    .ToListAsync(ct);

                var events = new List<(DateTimeOffset Time, int Count)>();

                foreach (var s in overlappingSchedules)
                {
                    events.Add((s.StartTime, s.StudentCount));   // Sinh viên vào
                    events.Add((s.EndTime, -s.StudentCount));    // Sinh viên ra
                }

                // 3. Thuật toán Sweeping Line để tìm đỉnh cao nhất (Peak)
                int peakStudents = 0;
                int currentStudents = 0;

                // Sắp xếp sự kiện theo thời gian, nếu trùng thời gian thì ưu tiên sự kiện "ra" trước để tránh vọt đỉnh ảo
                var sortedEvents = events.OrderBy(e => e.Time).ThenBy(e => e.Count).ToList();

                foreach (var ev in sortedEvents)
                {
                    currentStudents += ev.Count;
                    if (currentStudents > peakStudents)
                    {
                        peakStudents = currentStudents;
                    }
                }

                // 4. Kiểm tra với Capacity
                int projectedPeak = peakStudents + request.StudentCount;

                if (projectedPeak > room.Capacity)
                {
                    warningMessage = $"Cảnh báo: Tại thời điểm cao nhất, phòng {room.RoomNo} sẽ có {projectedPeak}/{room.Capacity} sinh viên.";
                }

                // 2. CHECK CONFLICT: Duyệt qua từng tuần để kiểm tra trùng lịch
                for (int i = 0; i < totalWeeks; i++)
                {
                    var weekStart = request.StartTime.AddDays(i * 7).ToUniversalTime();
                    var weekEnd = request.EndTime.AddDays(i * 7).ToUniversalTime();

                    // Kiểm tra lịch cá nhân của User (Schedules)
                    var hasScheduleConflict = await _unitOfWork.Repository<Schedule>().Entities
                        .AnyAsync(s => s.LecturerId == currentUserId && s.IsActive && !s.IsDeleted &&
                                       s.StartTime < weekEnd && s.EndTime > weekStart, ct);

                    // Kiểm tra các yêu cầu đặt chỗ khác của chính User này (tránh đặt 2 phòng cùng lúc)
                    var hasBookingConflict = await _unitOfWork.Repository<BookingRequest>().Entities
                        .AnyAsync(b => b.RequestedByUserId == currentUserId &&
                                       b.BookingRequestStatus == BookingRequestStatus.Pending &&
                                       b.Booking.StartTime < weekEnd && b.Booking.EndTime > weekStart, ct);

                    if (hasScheduleConflict || hasBookingConflict)
                        throw new BusinessException($"Trùng lịch vào tuần {i + 1} ({weekStart:dd/MM/yyyy}). Vui lòng kiểm tra lại.");
                }

                // 3. POLICY EVALUATION (Chỉ cần đánh giá cho tuần đầu tiên/request gốc)
                var activePolicies = room.RoomPolicies.Where(p => p.IsActive).ToList();
                await _policyEvaluator.EvaluateAsync(request, activePolicies);

                Guid firstBookingId = Guid.Empty;

                // 4. VÒNG LẶP TẠO CẶP (1 BOOKING : 1 BOOKING REQUEST)
                for (int i = 0; i < totalWeeks; i++)
                {
                    var bookingId = Guid.NewGuid();
                    if (i == 0) firstBookingId = bookingId;

                    var booking = new Booking
                    {
                        Id = bookingId,
                        LabRoomId = request.LabRoomId,
                        SlotTypeId = request.SlotTypeId > 0 ? request.SlotTypeId : null,
                        StartTime = request.StartTime.AddDays(i * 7).ToUniversalTime(),
                        EndTime = request.EndTime.AddDays(i * 7).ToUniversalTime(),
                        Recur = totalWeeks,
                        BookingStatus = BookingStatus.PendingApproval,
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
                        BookingId = bookingId, // Quan hệ 1:1
                        RequestedByUserId = currentUserId,
                        BookingRequestStatus = BookingRequestStatus.Pending,
                        CreatedAt = DateTimeOffset.UtcNow,
                        CreatedBy = currentUserId
                    };

                    await _unitOfWork.Repository<Booking>().AddAsync(booking);
                    await _unitOfWork.Repository<BookingRequest>().AddAsync(bookingRequest);
                }

                // 5. THÔNG BÁO DUY NHẤT (Chỉ gửi 1 Notification cho cả hành động này)
                var metadata = JsonSerializer.Serialize(new
                {
                    bookingId = firstBookingId,
                    totalWeeks = totalWeeks,
                    isRecurring = totalWeeks > 1
                });

                await _unitOfWork.Repository<Notification>().AddAsync(new Notification
                {
                    UserId = currentUserId,
                    Title = totalWeeks > 1 ? "Đặt lịch định kỳ thành công" : "Đặt lịch thành công",
                    Message = totalWeeks > 1
                        ? $"Bạn đã gửi yêu cầu đặt phòng {room.RoomName} lặp lại trong {totalWeeks} tuần."
                        : $"Yêu cầu đặt phòng {room.RoomName} của bạn đã được gửi.",
                    Type = "BookingCreated",
                    Metadata = JsonDocument.Parse(metadata).RootElement.Clone(),
                    IsRead = false,
                    IsGlobal = false,
                    CreatedAt = DateTimeOffset.UtcNow
                });

                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync();

                if (currentUserId != Guid.Empty)
                {
                    await _notificationService.NotifyBookingChangedAsync(currentUserId, new
                    {
                        action = "created",
                        bookingId = firstBookingId,
                        roomId = request.LabRoomId,
                        occurredAt = DateTimeOffset.UtcNow
                    }, ct);
                }

                // 6. TRICK: Chỉ publish event cho bản ghi đầu tiên để tránh spam Email/Event
                await _mediator.Publish(new BookingCreatedEvent(firstBookingId), ct);

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
