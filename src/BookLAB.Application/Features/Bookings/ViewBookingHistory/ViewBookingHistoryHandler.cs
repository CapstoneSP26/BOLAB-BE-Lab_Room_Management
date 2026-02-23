using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BookLAB.Application.Features.Bookings.ViewBookingHistory
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
            Guid Id = Guid.Parse(request.userId);
            return await _bookingService.GetBookingHistoryByUserIdAsync(Id, request.page, request.limit, request.status, request.startDate, request.endDate);
        }
    }
}
