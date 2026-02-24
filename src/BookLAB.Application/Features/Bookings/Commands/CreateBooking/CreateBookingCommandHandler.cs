using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPolicyEngine _policyEngine;
        private readonly ICurrentUserService _currentUserService;

        public CreateBookingCommandHandler(
        IUnitOfWork unitOfWork,
        IPolicyEngine policyEngine,
        ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _policyEngine = policyEngine;
            _currentUserService = currentUserService;
        }

        public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            // 1. check the existence of LabRoom
            var room = await _unitOfWork.Repository<LabRoom>().Entities
                .Include(r => r.RoomPolicies)
                .FirstOrDefaultAsync(r => r.Id == request.LabRoomId, cancellationToken);
            if (room == null || !room.IsActive)
                throw new NotFoundException("LabRoom is not existed or inactive");

            var currentUserId = _currentUserService.UserId ?? Guid.Empty;

            // 2. USER CONFLICT CHECK: Check if the user is already booked elsewhere
            var isUserBusy = await _unitOfWork.Repository<Schedule>().Entities
                .AnyAsync(s => s.LecturerId == currentUserId &&
                               s.IsActive && !s.IsDeleted &&
                               s.StartTime < request.EndTime &&
                               s.EndTime > request.StartTime, cancellationToken);

            if (isUserBusy)
            {
                throw new BusinessException("You already have another confirmed schedule during this time period.");
            }

            // 3. ROOM CAPACITY & OCCUPANCY CHECK (PRE-VALIDATION)
            // Fetch active schedules for this room to calculate remaining capacity
            var activeSchedules = await _unitOfWork.Repository<Schedule>().Entities
                .Where(s => s.LabRoomId == room.Id &&
                            s.IsActive && !s.IsDeleted &&
                            s.StartTime < request.EndTime &&
                            s.EndTime > request.StartTime)
                .ToListAsync(cancellationToken);

            // Validate occupancy count (Single vs Multi Occupancy)
            if (room.OverrideNumber <= activeSchedules.Count && room.OverrideNumber > 0)
            {
                throw new BusinessException($"{room.RoomName} has reached its maximum group limit ({room.OverrideNumber}).");
            }

            // Validate student capacity
            int currentStudents = activeSchedules.Sum(s => s.StudentCount);
            if (currentStudents + request.StudentCount > room.Capacity)
            {
                throw new BusinessException($"Not enough capacity in {room.RoomName}. Required: {request.StudentCount}, Available: {room.Capacity - currentStudents}.");
            }

            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                LabRoomId = request.LabRoomId,
                SlotTypeId = request.SlotTypeId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Recur = request.RecurringCount,
                BookingStatus = BookingStatus.PendingApproval,
                BookingType = request.BookingType,
                PurposeTypeId = request.PurposeTypeId,
                Reason = request.Reason
            };
            // 4. POLICY VALIDATION
            var activePolicies = room.RoomPolicies.Where(p => p.IsActive).ToList();
            await _policyEngine.ValidateAsync(booking, activePolicies, cancellationToken);

            // start transaction
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Repository<Booking>().AddAsync(booking);

                // Create the BookingRequest (The formal request for approval)
                var bookingRequest = new BookingRequest
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    RequestedByUserId = currentUserId,
                    BookingRequestStatus = BookingRequestStatus.Pending
                };
                await _unitOfWork.Repository<BookingRequest>().AddAsync(bookingRequest);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                return booking.Id;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

        }

    }
}
