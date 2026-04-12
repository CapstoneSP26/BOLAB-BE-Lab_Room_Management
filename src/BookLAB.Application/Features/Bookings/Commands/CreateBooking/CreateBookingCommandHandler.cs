using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
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
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPolicyEvaluator _policyEvaluator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;

        public CreateBookingCommandHandler(
        IUnitOfWork unitOfWork,
        IPolicyEvaluator policyEvaluator,
        ICurrentUserService currentUserService,
        IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _policyEvaluator = policyEvaluator;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<Guid> Handle(CreateBookingCommand request, CancellationToken ct)
        {
            // 0. Validate cơ bản trước khi đụng vào DB
            if (request.StartTime >= request.EndTime)
                throw new BusinessException("Start time must be before end time.");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 1. check the existence of LabRoom
                var room = await _unitOfWork.Repository<LabRoom>().Entities
            .Include(r => r.RoomPolicies)
            .FirstOrDefaultAsync(r => r.Id == request.LabRoomId, ct);

                if (room == null || !room.IsActive)
                    throw new NotFoundException("LabRoom is not existed or inactive");

                var currentUserId = _currentUserService.UserId ?? Guid.Empty;

                // 2. USER CONFLICT CHECK: Check if the user is already booked elsewhere
                var isUserBusySchedule = await _unitOfWork.Repository<Schedule>().Entities
                    .AnyAsync(s => s.LecturerId == currentUserId &&
                                   s.IsActive && !s.IsDeleted &&
                                   s.StartTime < request.EndTime &&
                                   s.EndTime > request.StartTime);
                var isUserBusyBooking = await _unitOfWork.Repository<BookingRequest>().Entities
                    .AnyAsync(b => b.RequestedByUserId == currentUserId &&
                                   b.BookingRequestStatus == BookingRequestStatus.Pending &&
                                   b.Booking.StartTime < request.EndTime &&
                                   b.Booking.EndTime > request.StartTime);


                if (isUserBusySchedule || isUserBusyBooking)
                {
                    throw new BusinessException("You already have another confirmed schedule during this time period.");
                }

                // 4. POLICY VALIDATION
                var activePolicies = room.RoomPolicies.Where(p => p.IsActive).ToList();

                await _policyEvaluator.EvaluateAsync(request, activePolicies);

                // start transaction

                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    LabRoomId = request.LabRoomId,
                    SlotTypeId = request.SlotTypeId > 0 ? request.SlotTypeId : null,
                    StartTime = request.StartTime.ToUniversalTime(),
                    EndTime = request.EndTime.ToUniversalTime(),
                    Recur = request.RecurringCount,
                    BookingStatus = BookingStatus.PendingApproval,
                    BookingType = request.BookingType,
                    PurposeTypeId = request.PurposeTypeId,
                    Reason = request.Reason,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = currentUserId
                };


                await _unitOfWork.Repository<Booking>().AddAsync(booking);

                // Create the BookingRequest (The formal request for approval)
                var bookingRequest = new BookingRequest
                {
                    Id = Guid.NewGuid(),
                    BookingId = booking.Id,
                    RequestedByUserId = currentUserId,
                    BookingRequestStatus = BookingRequestStatus.Pending,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = currentUserId
                };
                await _unitOfWork.Repository<BookingRequest>().AddAsync(bookingRequest);
                var metadataObject = new { bookingId = booking.Id};
                var metadataJsonString = JsonSerializer.Serialize(metadataObject);
                await _unitOfWork.Repository<Notification>().AddAsync(new Notification
                {
                    UserId = currentUserId,
                    Title = "Booking request submitted",
                    Message = $"Your booking request for room {room.RoomName} has been submitted and is waiting for approval.",
                    Type = "BookingCreated",
                    IsRead = false,
                    CreatedAt = DateTimeOffset.UtcNow,
                    Metadata = JsonDocument.Parse(metadataJsonString).RootElement.Clone(),
                    IsGlobal = false
                });

                await _unitOfWork.SaveChangesAsync(ct);
                await _unitOfWork.CommitTransactionAsync();
                await _mediator.Publish(new BookingCreatedEvent(booking.Id), ct);

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
