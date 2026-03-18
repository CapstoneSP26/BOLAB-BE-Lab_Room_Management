using FluentValidation;

namespace BookLAB.Application.Features.Bookings.Commands.UpdateCalendarEvent;

public class UpdateCalendarEventCommandValidator : AbstractValidator<UpdateCalendarEventCommand>
{
    public UpdateCalendarEventCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required.");
    }
}
