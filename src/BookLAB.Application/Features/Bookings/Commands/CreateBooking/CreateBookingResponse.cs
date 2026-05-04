namespace BookLAB.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingResponse
    {
        public Guid BookingId { get; set; }
        public string? WarningMessage { get; set; }
        public bool HasWarning => !string.IsNullOrEmpty(WarningMessage);
    }
}
