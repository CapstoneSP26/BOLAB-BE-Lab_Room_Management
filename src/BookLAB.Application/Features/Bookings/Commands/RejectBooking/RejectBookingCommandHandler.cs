using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Bookings.Commands.RejectBooking
{
    public class RejectBookingCommandHandler : IRequestHandler<RejectBookingCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public RejectBookingCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
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
          

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

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
