using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.GetBookingInAttendance
{
    public class BookingAttendance
    {
        public string? SearchTerm { get; set; }
        public string? Status { get; set; }
        public int? LabRoomId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? RequestedBy { get; set; }

        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public string? SortBy { get; set; }
        public bool? IsDescending { get; set; }
    }
}
