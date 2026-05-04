using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Domain.Enums;

namespace BookLAB.Application.Common.Policies.Handlers
{
    public class MinBookingLeadTimeHandler : IPolicyHandler
    {
        public PolicyType PolicyType => PolicyType.MinBookingLeadTime;

        public Task<PolicyValidationResult> ValidateAsync(CreateBookingCommand request, string value)
        {
            string defaultValue = "5"; // Default: phải đặt trước ít nhất 5 giờ

            if (!string.IsNullOrWhiteSpace(value))
            {
                defaultValue = value.Trim();
            }

            if (double.TryParse(defaultValue, out var minHours))
            {
                // Giờ tối thiểu cho phép (UTC)
                var minAllowedStartUtc = DateTime.UtcNow.AddHours(minHours);

                // request.StartTime hiện tại là giờ local/VN -> convert sang UTC trước khi so
                var requestStartUtc = request.StartTime.ToUniversalTime();

                if (requestStartUtc < minAllowedStartUtc)
                {
                    return Task.FromResult(
                        new PolicyValidationResult(
                            false,
                            $"Yêu cầu đặt trước ít nhất {minHours} giờ."
                        )
                    );
                }
            }

            return Task.FromResult(new PolicyValidationResult(true));
        }
    }
}