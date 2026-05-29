namespace BookLAB.Domain.Enums
{
    public enum EmailType
    {
        BookingApproved = 1,
        BookingRejected = 2,
        BookingCancelled = 3,
        BookingSubmitted = 4,
        BookingReminder = 5,
        StudentNotification = 6,
        RejectedByPriority = 7,
        BookingCancelledByOwner = 8,
        BookingCancelledByAdmin = 9,
        BookingRecovered = 10,
        NotifyAdminBookingCancelledByOwner = 11,
    }
}
