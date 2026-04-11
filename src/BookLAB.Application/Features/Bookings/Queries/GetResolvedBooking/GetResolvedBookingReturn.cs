using BookLAB.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.GetResolvedBooking
{
    public class GetResolvedBookingReturn
    {
        public List<ViewBookingHistoryResponseDTO> List { get; set; }
        public int total { get; set; }
    }
}
