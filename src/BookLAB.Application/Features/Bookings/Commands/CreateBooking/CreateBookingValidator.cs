using FluentValidation;

namespace BookLAB.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingValidator()
        {
            RuleFor(x => x.LabRoomId).NotEmpty();
            RuleFor(x => x.PurposeTypeId).NotEmpty();
            RuleFor(x => x.StartTime).GreaterThan(DateTime.UtcNow)
                .WithMessage("Start time must be in the future");
            RuleFor(x => x.EndTime).GreaterThan(x => x.StartTime)
                .WithMessage("End time must go after start time");
            RuleFor(x => x.Reason).MaximumLength(500);
        }
    }
}
