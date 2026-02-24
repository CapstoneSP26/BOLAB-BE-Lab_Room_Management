using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest
{
    public class ViewUncheckedBookingRequestValidator : AbstractValidator<ViewUncheckedBookingRequestCommand>
    {
        public ViewUncheckedBookingRequestValidator() 
        { 
            RuleFor(x => x.userId)
                .NotEmpty().WithMessage("User ID is required.")
                .Must(id => Guid.TryParse(id, out _)).WithMessage("User ID must be a valid GUID.");
        }
    }
}
