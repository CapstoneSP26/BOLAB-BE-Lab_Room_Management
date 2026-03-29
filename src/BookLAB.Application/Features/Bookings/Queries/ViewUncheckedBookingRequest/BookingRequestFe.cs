using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest
{
    public class BookingRequestFe
    {
        public Guid Id { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string BuildingName { get; set; }
        public Guid RequestedBy { get; set; }
        public DateTimeOffset RequestedAt { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Date { get; set; }
        public int? StudentCount { get; set; }
        public string Status { get; set; }
        public string Purpose { get; set; }
    }
}
