using BookLAB.Application.Common.Events;
using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Bookings.Commands.ApproveBooking
{
    public class ApproveBookingCommandHandler : IRequestHandler<ApproveBookingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ApproveBookingCommandHandler(IUnitOfWork unitOfWork, IMediator mediator, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _currentUserService = currentUserService;
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

            // 5. ROOM CAPACITY & OCCUPANCY CHECK
            var activeSchedules = await _unitOfWork.Repository<Schedule>().Entities
                .Where(s => s.LabRoomId == booking.LabRoomId &&
                    s.IsActive &&
                    !s.IsDeleted &&
                    s.StartTime < booking.EndTime &&
                    s.EndTime > booking.StartTime)
                .ToListAsync(cancellationToken);
            // Validate occupancy count (Single vs Multi Occupancy)
            if (booking.LabRoom.OverrideNumber <= activeSchedules.Count && booking.LabRoom.OverrideNumber > 0)
            {
                throw new BusinessException($"{booking.LabRoom.RoomName} has reached its maximum group limit ({booking.LabRoom.OverrideNumber}).");
            }

            // Validate student capacity
            int currentStudents = activeSchedules.Sum(s => s.StudentCount);
            if (currentStudents + booking.StudentCount > booking.LabRoom.Capacity)
            {
                throw new BusinessException($"Not enough capacity in {booking.LabRoom.RoomName}. Required: {booking.StudentCount}, Available: {booking.LabRoom.Capacity - currentStudents}.");
            }

            var bookingRequest = await _unitOfWork.Repository<BookingRequest>().Entities
                .FirstOrDefaultAsync(x => x.BookingId == booking.Id, cancellationToken);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (bookingRequest != null)
                {
                    bookingRequest.BookingRequestStatus = BookingRequestStatus.Approved;
                    _unitOfWork.Repository<BookingRequest>().Update(bookingRequest);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

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
