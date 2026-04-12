using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest
{
    public class ViewUncheckedBookingRequestReturn
    {
        public int total { get; set; }
        public List<BookingRequestFe> list { get; set; }
    }
}
