using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory
{
    public class ViewBookingHistoryValidator : AbstractValidator<ViewBookingHistoryCommand>
    {
        public ViewBookingHistoryValidator()
        {
            RuleFor(x => x.userId)
                .NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.page)
                .NotEmpty().WithMessage("Page number is required.")
                .GreaterThan(0).WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.limit)
                .NotEmpty().WithMessage("Limit is required.")
                .GreaterThan(0).WithMessage("Limit must be greater than 0.");

            RuleFor(x => x.status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(status => status == "all" || status == "confirmed" || status == "pending" || status == "cancelled" || status == "completed")
                .WithMessage("Status must be 'all', 'confirmed', 'pending', 'cancelled', 'completed'.");

            RuleFor(x => x.startDate)
                .LessThanOrEqualTo(x => x.endDate)
                .WithMessage("Start date must be less than or equal to end date.");
        }
    }
}
