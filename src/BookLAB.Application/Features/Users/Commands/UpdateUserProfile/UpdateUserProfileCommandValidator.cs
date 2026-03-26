using FluentValidation;

namespace BookLAB.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");

            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("Full name is required")
                .MaximumLength(100)
                .WithMessage("Full name must not exceed 100 characters");

            RuleFor(x => x.UserImageUrl)
                .MaximumLength(2048)
                .WithMessage("User image URL must not exceed 2048 characters")
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("User image URL must be a valid URL");
        }
    }
}
