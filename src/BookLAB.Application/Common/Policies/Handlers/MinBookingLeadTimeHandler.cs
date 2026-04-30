
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
            string defaultValue = "5"; // default curfew time
            if (!string.IsNullOrEmpty(value))
            {
                defaultValue = value.Trim();
            }

            if (double.TryParse(defaultValue, out var minHours))
            {
                var minAllowedStart = DateTime.UtcNow.AddHours(minHours);
                if (request.StartTime < minAllowedStart)
                    return Task.FromResult(new PolicyValidationResult(false, $"Yêu cầu đặt trước ít nhất {minHours} giờ."));
            }
            return Task.FromResult(new PolicyValidationResult(true));
        }
    }
}
