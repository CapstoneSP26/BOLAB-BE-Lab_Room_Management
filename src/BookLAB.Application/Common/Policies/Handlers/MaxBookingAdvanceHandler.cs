using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Domain.Enums;

namespace BookLAB.Application.Common.Policies.Handlers
{
    public class MaxBookingAdvanceHandler : IPolicyHandler
    {
        public PolicyType PolicyType => PolicyType.MaxBookingAdvance;

        public Task<PolicyValidationResult> ValidateAsync(CreateBookingCommand request, string value)
        {
            if (double.TryParse(value, out var maxDays))
            {
                var maxAllowedDate = DateTime.UtcNow.AddDays(maxDays);
                if (request.StartTime > maxAllowedDate)
                    return Task.FromResult(new PolicyValidationResult(false, $"Chỉ được phép đặt trước tối đa {maxDays} ngày."));
            }
            return Task.FromResult(new PolicyValidationResult(true));
        }
    }
}