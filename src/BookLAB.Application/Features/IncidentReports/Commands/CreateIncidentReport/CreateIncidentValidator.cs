using FluentValidation;

namespace BookLAB.Application.Features.IncidentReports.Commands.CreateIncidentReport
{
    public class CreateIncidentValidator : AbstractValidator<CreateIncidentCommand>
    {
        public CreateIncidentValidator()
        {
            RuleFor(x => x.ScheduleId)
                .NotEmpty();

            RuleFor(x => x.CreatedBy)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(4000);
        }
    }
}