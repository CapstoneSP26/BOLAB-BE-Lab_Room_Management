using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Integration;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Bookings.Events;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BookLAB.Application.Features.Bookings.Commands.ApproveBooking
{
    public class ApproveBookingCommandHandler : IRequestHandler<ApproveBookingCommand, bool>
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

        public async Task<bool> Handle(ApproveBookingCommand request, CancellationToken cancellationToken)
        {
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

            if (isUserBusy)
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
            int currentStudents = activeSchedules.Sum(s => s.StudentCount);
            if (currentStudents + booking.StudentCount > booking.LabRoom.Capacity)
            {
                throw new BusinessException($"Not enough capacity in {booking.LabRoom.RoomName}. Required: {booking.StudentCount}, Available: {booking.LabRoom.Capacity - currentStudents}.");
            }

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
                _unitOfWork.Repository<BookingRequest>().Update(bookingRequest);

                booking.BookingStatus = BookingStatus.Approved;
                _unitOfWork.Repository<Booking>().Update(booking);

                Notification? createdNotification = null;
                var metadataObject = new { bookingId = booking.Id, labName = "Lab 01" };
                var metadataJsonString = JsonSerializer.Serialize(metadataObject);
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

                // throw event to notify other parts of the system that a booking has been approved
                await _mediator.Publish(new BookingApprovedEvent(booking.Id, currentUserId), cancellationToken);

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
