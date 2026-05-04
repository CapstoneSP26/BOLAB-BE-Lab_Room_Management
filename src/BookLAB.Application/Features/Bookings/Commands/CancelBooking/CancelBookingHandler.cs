using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Integration;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace BookLAB.Application.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingHandler : IRequestHandler<CancelBookingCommand, ResultMessage<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationService _notificationService;
        public CancelBookingHandler(IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _notificationService = notificationService;
        }

        public async Task<ResultMessage<bool>> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (!(await _unitOfWork.Repository<Booking>().Entities.AnyAsync(x => x.CreatedBy == userId && x.Id == request.BookingId)))
                return new ResultMessage<bool>
                {
                    Success = false,
                    Message = "You are not the owner of this booking.",
                };

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(request.BookingId);

                if (booking == null)
                    return new ResultMessage<bool>
                    {
                        Success = false,
                        Message = "Booking not found.",
                    };

                var managerNotifications = new List<Notification>();
                var metadataObject = new { bookingId = booking.Id, labRoomId = booking.LabRoomId };
                var metadataJsonString = JsonSerializer.Serialize(metadataObject);

                var ownerIds = await _unitOfWork.LabOwners.GetOwnerIdsByLabRoomIdAsync(booking.LabRoomId);
                foreach (var managerId in ownerIds.Distinct())
                {
                    if (userId.HasValue && managerId == userId.Value)
                    {
                        continue;
                    }

                    var managerNotification = new Notification
                    {
                        UserId = managerId,
                        Title = "Booking cancelled",
                        Message = $"Booking {booking.Id} was cancelled by the requester.",
                        Type = "BookingCancelled",
                        IsRead = false,
                        CreatedAt = DateTimeOffset.UtcNow,
                        Metadata = JsonDocument.Parse(metadataJsonString).RootElement.Clone(),
                        IsGlobal = false
                    };

                    managerNotifications.Add(managerNotification);
                    await _unitOfWork.Repository<Notification>().AddAsync(managerNotification);
                }

                _unitOfWork.Repository<Booking>().Delete(booking);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                if (userId is Guid ownerId)
                {
                    await _notificationService.NotifyBookingChangedAsync(ownerId, new
                    {
                        action = "cancelled",
                        bookingId = request.BookingId,
                        occurredAt = DateTimeOffset.UtcNow
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

                return new ResultMessage<bool>
                {
                    Success = true,
                    Message = "Booking cancelled successfully.",
                    Data = true
                };
            } catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ResultMessage<bool>
                {
                    Success = false,
                    Message = "An error occurred while cancelling the booking.",
                    Data = false
                };
            }
            
        }
    }
}
