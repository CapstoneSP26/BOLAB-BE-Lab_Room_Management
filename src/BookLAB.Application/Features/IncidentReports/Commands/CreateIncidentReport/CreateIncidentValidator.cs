using FluentValidation;

namespace BookLAB.Application.Features.IncidentReports.Commands.CreateIncidentReport
{
    public class CreateIncidentValidator : AbstractValidator<CreateIncidentCommand>
    {
        public CreateIncidentValidator()
        {
            RuleFor(x => x.ReportedBy)
                .NotEmpty();

            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .NotEmpty()
                .MaximumLength(4000);

            RuleFor(x => x.Severity)
                .IsInEnum();

            RuleFor(x => x.Environment)
                .MaximumLength(200);

            RuleFor(x => x.ExpectedResult)
                .MaximumLength(2000);

            RuleFor(x => x.ActualResult)
                .MaximumLength(2000);

            RuleFor(x => x.AttachmentUrl)
                .MaximumLength(2048)
                .Must(BeAValidAbsoluteUrl)
                .When(x => !string.IsNullOrWhiteSpace(x.AttachmentUrl))
                .WithMessage("AttachmentUrl must be a valid absolute URL.");

            RuleFor(x => x.StepsToReproduce)
                .Must(steps => steps.Count <= 20)
                .WithMessage("StepsToReproduce cannot contain more than 20 items.");

            RuleForEach(x => x.StepsToReproduce)
                .NotEmpty()
                .MaximumLength(1000);
        }

        private static bool BeAValidAbsoluteUrl(string? value)
        {
            return Uri.TryCreate(value, UriKind.Absolute, out _);
        }
    }
}