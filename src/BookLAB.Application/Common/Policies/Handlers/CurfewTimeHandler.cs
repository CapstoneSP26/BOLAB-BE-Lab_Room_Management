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
            string defaultValue = "23:59"; // Default giờ đóng cửa

            if (!string.IsNullOrWhiteSpace(value))
            {
                defaultValue = value.Trim();
            }

            if (TimeOnly.TryParse(defaultValue, out var curfew))
            {
                // Convert UTC/local về giờ Việt Nam trước khi kiểm tra giờ trong ngày
                var bookingEndVietnamTime = request.EndTime.ToLocalTime();

                var bookingEndTime = TimeOnly.FromDateTime(bookingEndVietnamTime);

                if (bookingEndTime > curfew)
                {
                    return Task.FromResult(
                        new PolicyValidationResult(
                            false,
                            $"Phòng Lab đóng cửa lúc {curfew}."
                        )
                    );
                }
            }

            return Task.FromResult(new PolicyValidationResult(true));
        }
    }
}