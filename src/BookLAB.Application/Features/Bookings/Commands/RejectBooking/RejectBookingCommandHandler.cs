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

namespace BookLAB.Application.Features.Bookings.Commands.RejectBooking
{
    public class RejectBookingCommandHandler : IRequestHandler<RejectBookingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private readonly INotificationService _notificationService;

        public RejectBookingCommandHandler(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IMediator mediator,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mediator = mediator;
            _notificationService = notificationService;
        }

        public async Task<bool> Handle(RejectBookingCommand request, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId);
            if (booking == null)
                throw new NotFoundException(nameof(Booking), request.BookingId);

            var currentUserId = _currentUserService.UserId ?? Guid.Empty;
            var isOwner = await _unitOfWork.LabOwners.IsUserOwnerAsync(booking.LabRoomId, currentUserId);

            if (!isOwner)
                throw new ForbiddenException("You do not have right to manipulate on this booking");

            if (booking.BookingStatus != BookingStatus.PendingApproval)
                throw new BusinessException("This booking is not in pending status");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var bookingRequest = await _unitOfWork.Repository<BookingRequest>().Entities
                   .FirstOrDefaultAsync(x => x.BookingId == booking.Id, cancellationToken);
                if (bookingRequest == null)
                {
                    throw new NotFoundException(nameof(BookingRequest), booking.Id);
                }

                booking.BookingStatus = BookingStatus.Rejected;
                _unitOfWork.Repository<Booking>().Update(booking);

                bookingRequest.BookingRequestStatus = BookingRequestStatus.Rejected;
                bookingRequest.ResponseContext = request.Reason; 
                _unitOfWork.Repository<BookingRequest>().Update(bookingRequest);

                Notification? createdNotification = null;
                var metadataObject = new { bookingId = booking.Id, labName = "Lab 01" };
                var metadataJsonString = JsonSerializer.Serialize(metadataObject);
                if (booking.CreatedBy.HasValue)
                {
                    createdNotification = new Notification
                    {
                        UserId = booking.CreatedBy.Value,
                        Title = "Booking rejected",
                        Message = $"Your booking request has been rejected. Reason: {request.Reason}",
                        Type = "BookingRejected",
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

                await _mediator.Publish(new BookingRejectedEvent(booking.Id), cancellationToken);

                return true;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}
