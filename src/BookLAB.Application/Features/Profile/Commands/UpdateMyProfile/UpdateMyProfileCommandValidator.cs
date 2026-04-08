using FluentValidation;

namespace BookLAB.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommandValidator : AbstractValidator<UpdateMyProfileCommand>
{
    public UpdateMyProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(255)
            .When(x => !string.IsNullOrWhiteSpace(x.FullName))
            .WithMessage("Full name must not exceed 255 characters");

        RuleFor(x => x.Email)
            .EmailAddress()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .WithMessage("Email must be a valid email address");
    }
}
