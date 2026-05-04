using FluentValidation;

namespace BookLAB.Application.Features.Bookings.Commands.DeleteCalendarEvent;

public class DeleteCalendarEventCommandValidator : AbstractValidator<DeleteCalendarEventCommand>
{
    public DeleteCalendarEventCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty()
            .WithMessage("Booking ID is required.");
    }
}
