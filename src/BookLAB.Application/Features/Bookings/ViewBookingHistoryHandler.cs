using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.Bookings
{
    public class ViewBookingHistoryHandler : IRequestHandler<ViewBookingHistoryCommand, List<Booking>>
    {
        private readonly IBookingService _bookingService;

        public ViewBookingHistoryHandler(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        public async Task<List<Booking>> Handle(ViewBookingHistoryCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId == Guid.Empty)
            {
                throw new ArgumentException("Invalid UserId");
            }

            return await _bookingService.GetBookingHistoryByUserIdAsync(request.UserId);
        }
    }
}
