using System;

namespace BookLAB.Application.Features.Bookings.Commands.CreateBooking
{
    public class BookingSummary
    {
        public string Id { get; set; }
        public string RoomName { get; set; }
        public string Building { get; set; }
        public string Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public bool RepeatWeekly { get; set; }
        public string? WeeklyUntil { get; set; }
        public string? GroupName { get; set; }
        public BookLAB.Domain.Enums.BookingStatus Status { get; set; }
        public string CreatedAt { get; set; }
    }
}
