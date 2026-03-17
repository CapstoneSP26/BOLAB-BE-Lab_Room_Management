using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LoginWithGoogle
{
    public class LoginWithGoogleValidator : AbstractValidator<LoginWithGoogleCommand>
    {
        public LoginWithGoogleValidator()
        {
            RuleFor(x => x.IdToken)
                .NotEmpty().WithMessage("Google ID Token is required.");
        }
    }
}
