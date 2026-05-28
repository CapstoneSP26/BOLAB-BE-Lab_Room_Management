using BookLAB.Application.Common.Extensions;
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
                // Giả sử logic hệ thống tính theo giờ hiện tại (UTC)
                var nowUtc = DateTime.UtcNow;

                // 1. Mốc tối đa dựa theo số ngày cấu hình (Lấy đến cuối ngày đó)
                var maxAdvanceDateUtc = nowUtc.AddDays(maxDays).Date.AddDays(1).AddTicks(-1);

                // 2. Mốc tối đa dựa theo ngày kết thúc học kỳ hiện tại
                var semesterEndDateUtc = DateTimeExtensions.GetCurrentSemesterEndDateUtc(nowUtc);

                // 3. Chọn mốc thời gian nhỏ hơn (sớm hơn) làm giới hạn chặn trên
                var maxAllowedDateUtc = maxAdvanceDateUtc < semesterEndDateUtc
                    ? maxAdvanceDateUtc
                    : semesterEndDateUtc;

                // Convert giờ request sang UTC trước khi so sánh
                var requestStartUtc = request.StartTime.ToUniversalTime();

                if (requestStartUtc > maxAllowedDateUtc)
                {
                    // Trả về thông báo tương ứng với lý do bị chặn
                    if (maxAdvanceDateUtc < semesterEndDateUtc)
                    {
                        return Task.FromResult(
                            new PolicyValidationResult(
                                false,
                                $"Chỉ được phép đặt trước tối đa {maxDays} ngày."
                            )
                        );
                    }
                    else
                    {
                        return Task.FromResult(
                            new PolicyValidationResult(
                                false,
                                "Bạn chỉ được phép đặt các ngày nằm trong học kỳ hiện tại."
                            )
                        );
                    }
                }
            }

            return Task.FromResult(new PolicyValidationResult(true));
        }
    }
}