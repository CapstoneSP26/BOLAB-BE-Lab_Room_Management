using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Integration;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
