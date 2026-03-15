using Google.Apis.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingRequest
    {
        public string roomId { get; set; }
        public string date { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public bool repeatWeekly { get; set; }
        public string? weeklyUntil { get; set; }
        public string? groupId { get; set; }
        public string? notes { get; set; }
    }
}
