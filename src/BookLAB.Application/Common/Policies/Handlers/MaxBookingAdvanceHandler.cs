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
            string defaultValue = "14"; // Default: đặt trước tối đa 14 ngày

            if (!string.IsNullOrWhiteSpace(value))
            {
                defaultValue = value.Trim();
            }

            if (double.TryParse(defaultValue, out var maxDays))
            {
                // Mốc tối đa được phép đặt (UTC)
                var maxAllowedDateUtc = DateTime.UtcNow.AddDays(maxDays);

                // Convert giờ request sang UTC trước khi so sánh
                var requestStartUtc = request.StartTime.ToUniversalTime();

                if (requestStartUtc > maxAllowedDateUtc)
                {
                    return Task.FromResult(
                        new PolicyValidationResult(
                            false,
                            $"Chỉ được phép đặt trước tối đa {maxDays} ngày."
                        )
                    );
                }
            }

            return Task.FromResult(new PolicyValidationResult(true));
        }
    }
}