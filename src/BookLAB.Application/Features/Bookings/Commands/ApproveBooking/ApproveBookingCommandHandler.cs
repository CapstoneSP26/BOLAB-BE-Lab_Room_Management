using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Integration;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Bookings.Events;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace BookLAB.Application.Features.Bookings.Commands.ApproveBooking
{
    public class ApproveBookingCommandHandler : IRequestHandler<ApproveBookingCommand, ApproveBookingResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;

        public ApproveBookingCommandHandler(
            IUnitOfWork unitOfWork,
            IMediator mediator,
            ICurrentUserService currentUserService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task<ApproveBookingResponse> Handle(ApproveBookingCommand request, CancellationToken cancellationToken)
        {
            var response = new ApproveBookingResponse();
            // 1. Fetch Booking with LabRoom details
            var booking = await _unitOfWork.Repository<Booking>().Entities
                .Include(b => b.LabRoom)
                .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

            if (booking == null) throw new NotFoundException(nameof(Booking), request.BookingId);

            // 2. Security Check: Only Lab Owners can approve
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;
            var isOwner = await _unitOfWork.LabOwners.IsUserOwnerAsync(booking.LabRoomId, currentUserId);
            if (!isOwner)
            {
                throw new ForbiddenException("You do not have right to manipulate on this booking");
            }

            // 3. Status Validation: Only PendingApproval state is allowed
            if (booking.BookingStatus != BookingStatus.PendingApproval)
                throw new BusinessException("This booking is not in pending status");

            // 4. USER CONFLICT CHECK: Ensure the requester isn't already scheduled elsewhere
            var isUserBusy = await _unitOfWork.Repository<Schedule>().Entities
                .AnyAsync(s => s.LecturerId == booking.CreatedBy &&
                       s.IsActive && !s.IsDeleted &&
                       s.StartTime < booking.EndTime &&
                       s.EndTime > booking.StartTime, cancellationToken);

            var bookingCreatedBy = await _unitOfWork.Repository<User>().Entities
                .AsNoTracking() 
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == booking.CreatedBy);
            var isAdmin = bookingCreatedBy.UserRoles.Any(ur => ur.RoleId == 1);

            if (isUserBusy && !isAdmin)
            {
                throw new BusinessException("The requester already has another confirmed schedule during this time period.");
            }

            // Get Max Concurrent Bookings Policy for the room
            var maxConcurrentBookingsPolicy = await  _unitOfWork.Repository<RoomPolicy>().Entities
                .Where(rp => rp.LabRoomId == booking.LabRoomId && rp.PolicyKey == PolicyType.MaxConcurrentBookings)
                .Select(rp => rp.PolicyValue)
                .FirstOrDefaultAsync(cancellationToken);
            var maxConcurrentBookings = int.TryParse(maxConcurrentBookingsPolicy, out var result) ? result : 1;

            // 5. ROOM CAPACITY & OCCUPANCY CHECK
            var activeSchedules = await _unitOfWork.Repository<Schedule>().Entities
                .Where(s => s.LabRoomId == booking.LabRoomId &&
                    s.IsActive &&
                    !s.IsDeleted &&
                    s.StartTime < booking.EndTime &&
                    s.EndTime > booking.StartTime)
                .ToListAsync(cancellationToken);
            // Validate occupancy count (Single vs Multi Occupancy)
            if (maxConcurrentBookings <= activeSchedules.Count)
            {
                throw new BusinessException($"{booking.LabRoom.RoomName} has reached its maximum group limit ({maxConcurrentBookings}).");
            }

            // Validate student capacity
            //int currentStudents = activeSchedules.Sum(s => s.StudentCount);
            //if (currentStudents + booking.StudentCount > booking.LabRoom.Capacity)
            //{
            //    throw new BusinessException($"Not enough capacity in {booking.LabRoom.RoomName}. Required: {booking.StudentCount}, Available: {booking.LabRoom.Capacity - currentStudents}.");
            //}

            

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var bookingRequest = await _unitOfWork.Repository<BookingRequest>().Entities
                    .FirstOrDefaultAsync(x => x.BookingId == booking.Id, cancellationToken);
                if (bookingRequest == null)
                {
                    throw new NotFoundException(nameof(BookingRequest), booking.Id);
                }

                bookingRequest.BookingRequestStatus = BookingRequestStatus.Approved;
                bookingRequest.ResponsedByUserId = _currentUserService.UserId;
                _unitOfWork.Repository<BookingRequest>().Update(bookingRequest);

                booking.BookingStatus = BookingStatus.Approved;
                _unitOfWork.Repository<Booking>().Update(booking);

                // ==================== LOGIC XỬ LÝ ĐÈ PRIORITY (CONFLICT RESOLUTION) ====================

                // 1. Quét và xử lý các SCHEDULE (Lịch đã duyệt) có Priority THẤP HƠN
                var overlappingSchedules = await _unitOfWork.Repository<Schedule>().Entities
                    .AsNoTracking() 
                    .Where(s => s.LabRoomId == booking.LabRoomId &&
                                s.IsActive && !s.IsDeleted &&
                                s.StartTime < booking.EndTime &&
                                s.EndTime > booking.StartTime) // Thấp hơn lịch đang duyệt
                    .ToListAsync(cancellationToken);
                // Kiểm tra xem mức độ ưu tiên sắp duyệt thuộc nhóm Độc Quyền (Mức >= 2: Academic/SchoolEvent) hay Chia sẻ (Normal)
                bool isNewBookingExclusive = booking.PurposeTypeId >= 2;
                if (isNewBookingExclusive)
                {
                    // ĐỘC QUYỀN: Nếu có BẤT KỲ lịch nào ĐÃ DUYỆT có priority BẰNG hoặc CAO HƠN đang chạy -> CHẶN
                    var higherOrEqualSchedule = overlappingSchedules
                        .FirstOrDefault(s => (int)s.SchedulePriority >= booking.PurposeTypeId);

                    if (higherOrEqualSchedule != null)
                        throw new BusinessException($"Cannot approve. This room is already occupied by a higher or equal priority schedule.");
                }
                else
                {
                    // CHIA SẺ (NORMAL):
                    // a. Nếu thời gian này ĐÃ CÓ lịch Độc quyền (Academic/SchoolEvent) được duyệt -> CHẶN THẲNG
                    var hasExclusiveSchedule = overlappingSchedules.Any(s => (int)s.SchedulePriority >= 2);
                    if (hasExclusiveSchedule)
                        throw new BusinessException("This room is already reserved for Academic or School Events during this period.");

                    // b. Kiểm tra chính sách đặt phòng đồng thời (Concurrent Policy)
                    var activeNormalSchedulesCount = overlappingSchedules.Count(s => s.SchedulePriority == SchedulePriority.NORMAL);
                    if (maxConcurrentBookings <= activeNormalSchedulesCount)
                        throw new BusinessException($"{booking.LabRoom.RoomName} has reached its maximum concurrent group limit ({maxConcurrentBookings}).");
                }

                var lowerPrioritySchedules = overlappingSchedules
                    .Where(s => (int)s.SchedulePriority < booking.PurposeTypeId)
                    .ToList();

                foreach (var oldSchedule in lowerPrioritySchedules)
                {
                    oldSchedule.IsActive = false; // Hoặc đổi sang trạng thái Cancelled tuỳ DB của bạn
                    oldSchedule.ScheduleStatus = ScheduleStatus.Cancelled;
                    oldSchedule.IsDeleted = true; // Nếu bạn muốn đánh dấu là đã huỷ và không còn hiệu lực
                    oldSchedule.AutoCancelledByBookingId = booking.Id;
                    _unitOfWork.Repository<Schedule>().Update(oldSchedule);
                    response.CancelledScheduleIds.Add(oldSchedule.Id);

                    // TODO: Bạn có thể thêm logic bắn notification riêng cho các đối tượng bị huỷ lịch ở đây
                }

                // 2. Quét và xử lý các BOOKING REQUEST (Đang chờ) có Priority THẤP HƠN
                var overlappingBookings = await _unitOfWork.Repository<Booking>().Entities
                    .AsNoTracking()
                    .Where(b => b.LabRoomId == booking.LabRoomId &&
                                b.Id != booking.Id && // Tránh chính nó
                                b.BookingStatus == BookingStatus.PendingApproval &&
                                b.StartTime < booking.EndTime &&
                                b.EndTime > booking.StartTime)
                    .ToListAsync(cancellationToken);

                var higherPriorityBookings = overlappingBookings
                    .Where(b => b.PurposeTypeId > booking.PurposeTypeId)
                    .ToList();

                if (higherPriorityBookings.Any())
                {
                    throw new BusinessException("Cannot approve this booking. There is another pending request with a higher priority in this time slot that must be processed first.");
                }

                var lowerPriorityBookings = new List<Booking>();
                if(booking.PurposeTypeId > 1)
                {
                    lowerPriorityBookings = overlappingBookings
                    .Where(b => b.PurposeTypeId <= booking.PurposeTypeId)
                    .ToList();
                }


                foreach (var lowBooking in lowerPriorityBookings)
                {
                    lowBooking.BookingStatus = BookingStatus.Rejected;
                    lowBooking.AutoRejectedByBookingId = booking.Id;
                    _unitOfWork.Repository<Booking>().Update(lowBooking);
                    response.RejectedBookingIds.Add(lowBooking.Id);

                    // Đồng bộ cập nhật luôn bảng phụ BookingRequest nếu có
                    var lowBookingReq = await _unitOfWork.Repository<BookingRequest>().Entities
                        .FirstOrDefaultAsync(x => x.BookingId == lowBooking.Id, cancellationToken);
                    if (lowBookingReq != null)
                    {
                        lowBookingReq.BookingRequestStatus = BookingRequestStatus.Rejected;
                        _unitOfWork.Repository<BookingRequest>().Update(lowBookingReq);
                    }
                }
                // =======================================================================================

                Notification? createdNotification = null;
                var metadataObject = new { bookingId = booking.Id, labName = "Lab 01" };
                var metadataJsonString = JsonSerializer.Serialize(metadataObject);
                var managerNotifications = new List<Notification>();
                if (booking.CreatedBy.HasValue)
                {
                    createdNotification = new Notification
                    {
                        UserId = booking.CreatedBy.Value,
                        Title = "Booking approved",
                        Message = $"Your booking for room {booking.LabRoom.RoomName} has been approved.",
                        Type = "BookingApproved",
                        IsRead = false,
                        CreatedAt = DateTimeOffset.UtcNow,
                        Metadata = JsonDocument.Parse(metadataJsonString).RootElement.Clone(),
                        IsGlobal = false
                    };

                    await _unitOfWork.Repository<Notification>().AddAsync(createdNotification);
                }

                var ownerIds = await _unitOfWork.LabOwners.GetOwnerIdsByLabRoomIdAsync(booking.LabRoomId);
                foreach (var ownerId in ownerIds.Distinct())
                {
                    if (booking.CreatedBy.HasValue && ownerId == booking.CreatedBy.Value)
                    {
                        continue;
                    }

                    var managerNotification = new Notification
                    {
                        UserId = ownerId,
                        Title = "Booking approved",
                        Message = $"Booking {booking.Id} for room {booking.LabRoom.RoomName} was approved.",
                        Type = "BookingApproved",
                        IsRead = false,
                        CreatedAt = DateTimeOffset.UtcNow,
                        Metadata = JsonDocument.Parse(metadataJsonString).RootElement.Clone(),
                        IsGlobal = false
                    };

                    managerNotifications.Add(managerNotification);
                    await _unitOfWork.Repository<Notification>().AddAsync(managerNotification);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                if (createdNotification?.UserId is Guid notificationUserId)
                {
                    await _notificationService.NotifyNotificationCreatedAsync(notificationUserId, new
                    {
                        id = createdNotification.Id,
                        type = createdNotification.Type,
                        title = createdNotification.Title,
                        message = createdNotification.Message,
                        isRead = createdNotification.IsRead,
                        createdAt = createdNotification.CreatedAt,
                        metadata = createdNotification.Metadata
                    }, cancellationToken);
                }

                foreach (var managerNotification in managerNotifications)
                {
                    if (managerNotification.UserId is not Guid managerUserId)
                    {
                        continue;
                    }

                    await _notificationService.NotifyNotificationCreatedAsync(managerUserId, new
                    {
                        id = managerNotification.Id,
                        type = managerNotification.Type,
                        title = managerNotification.Title,
                        message = managerNotification.Message,
                        isRead = managerNotification.IsRead,
                        createdAt = managerNotification.CreatedAt,
                        metadata = managerNotification.Metadata
                    }, cancellationToken);
                }

                if (booking.CreatedBy is Guid bookingOwnerId)
                {
                    await _notificationService.NotifyBookingChangedAsync(bookingOwnerId, new
                    {
                        action = "approved",
                        bookingId = booking.Id,
                        labRoomId = booking.LabRoomId,
                        status = booking.BookingStatus.ToString(),
                        occurredAt = DateTimeOffset.UtcNow
                    }, cancellationToken);
                }

                // throw event to notify other parts of the system that a booking has been approved
                await _mediator.Publish(new BookingApprovedEvent(booking.Id, currentUserId, response.RejectedBookingIds, response.CancelledScheduleIds), cancellationToken);

                // 3. Gọi SignalR Notify cho cả hệ thống
                var payload = new
                {
                    publisherId = currentUserId,
                    labRoomId = booking.LabRoomId,
                    startTime = booking.StartTime,
                    endTime = booking.EndTime,
                };

                // Gọi method bạn vừa viết
                await _notificationService.NotifyScheduleStatusChangedAsync(payload, cancellationToken);
                // Thiết lập dữ liệu trả về thành công
                response.Status = booking.BookingStatus.ToString();
                response.Message = $"Booking approved successfully. Cancelled {response.CancelledScheduleIds.Count} schedules and rejected {response.RejectedBookingIds.Count} pending requests due to priority override.";

                return response;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
