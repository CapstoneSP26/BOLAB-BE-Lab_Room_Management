namespace BookLAB.Application.Features.Bookings.Commands.ApproveBooking
{
    public class ApproveBookingResponse
    {
        public Guid BookingId { get; set; }
        public string Status { get; set; } = string.Empty;
        public List<Guid> CancelledScheduleIds { get; set; } = new List<Guid>();
        public List<Guid> RejectedBookingIds { get; set; } = new List<Guid>();
        public string Message { get; set; } = string.Empty;
    }
}
