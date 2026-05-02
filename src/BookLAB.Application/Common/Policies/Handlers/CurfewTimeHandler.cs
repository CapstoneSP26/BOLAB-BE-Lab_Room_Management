using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Domain.Enums;

namespace BookLAB.Application.Common.Policies.Handlers
{
    public class CurfewTimeHandler : IPolicyHandler
    {
        public PolicyType PolicyType => PolicyType.CurfewTime;

        public Task<PolicyValidationResult> ValidateAsync(CreateBookingCommand request, string value)
        {
            string defaultValue = "23:59"; // default curfew time
            if (!string.IsNullOrEmpty(value))
            {
                defaultValue = value.Trim();
            }

            if (TimeOnly.TryParse(defaultValue, out var curfew))
            {
                var bookingEndTime = TimeOnly.FromDateTime(request.EndTime);
                if (bookingEndTime > curfew)
                    return Task.FromResult(new PolicyValidationResult(false, $"Phòng Lab đóng cửa lúc {curfew}."));
            }
            return Task.FromResult(new PolicyValidationResult(true));
        }
    }
}
