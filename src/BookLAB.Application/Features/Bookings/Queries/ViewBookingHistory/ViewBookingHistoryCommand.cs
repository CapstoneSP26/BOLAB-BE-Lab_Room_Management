using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory
{
    public class ViewBookingHistoryCommand : IRequest<List<Booking>>
    {
        public Guid UserId { get; set; }
    }
}
