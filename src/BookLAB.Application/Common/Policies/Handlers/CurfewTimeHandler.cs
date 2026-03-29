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
            if (TimeOnly.TryParse(value, out var curfew))
            {
                var bookingEndTime = TimeOnly.FromDateTime(request.EndTime);
                if (bookingEndTime > curfew)
                    return Task.FromResult(new PolicyValidationResult(false, $"Phòng Lab đóng cửa lúc {curfew}."));
            }
            return Task.FromResult(new PolicyValidationResult(true));
        }
    }
}
