using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingResponse
    {
        public bool success { get; set; }
        public string bookingId { get; set; }
        public BookingSummary summary { get; set; }
        public string? message { get; set; }
    }
}
