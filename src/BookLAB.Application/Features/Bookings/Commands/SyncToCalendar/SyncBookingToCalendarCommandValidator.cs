using FluentValidation;

namespace BookLAB.Application.Features.Bookings.Commands.SyncToCalendar;

public class SyncBookingToCalendarCommandValidator : AbstractValidator<SyncBookingToCalendarCommand>
{
    public SyncBookingToCalendarCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required.");
    }
}
