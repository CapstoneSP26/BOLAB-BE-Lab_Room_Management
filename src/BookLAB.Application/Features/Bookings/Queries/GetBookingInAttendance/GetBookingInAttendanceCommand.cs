using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Queries.GetBookings;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.GetBookingInAttendance
{
    public class GetBookingInAttendanceCommand : IRequest<PagedList<GetBookings.BookingDto>> 
    {
        public GetBookingsQuery query { get; set; }
    }
}
