using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.GetBookingStats
{
    public class GetBookingStatsValidator : AbstractValidator<GetBookingStatsCommand>
    {
        public GetBookingStatsValidator()
        {
            RuleFor(x => x.userId).NotEmpty().WithMessage("User ID is required.");

            RuleFor(x => x.startDate).NotEmpty().WithMessage("Start date is required.");

            RuleFor(x => x.endDate).NotEmpty().WithMessage("End date is required.")
                .GreaterThanOrEqualTo(x => x.startDate).WithMessage("End date must be greater than or equal to start date.");
        }
    }
}
