using System.Globalization;

namespace BookLAB.Application.Common.Extensions
{
    public static class DateTimeExtensions
    {
        // Định nghĩa múi giờ Việt Nam (UTC+7)
        // Lưu ý: "SE Asia Standard Time" là ID chuẩn trên Windows. 
        // Nếu chạy trên Linux (Docker), ID thường là "Asia/Ho_Chi_Minh".
        private static readonly TimeZoneInfo VietnamTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        /// <summary>
        /// Chuyển đổi DateTimeOffset (thường là UTC từ DB) sang DateTime tại múi giờ VN
        /// </summary>
        public static DateTime ToVietnamTime(this DateTimeOffset dateTimeOffset)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(dateTimeOffset.UtcDateTime, VietnamTimeZone);
        }

        /// <summary>
        /// Convert sang giờ Việt Nam (giữ DateTimeOffset)
        /// </summary>
        public static DateTimeOffset ToVietnamOffset(this DateTimeOffset dateTimeOffset)
        {
            return TimeZoneInfo.ConvertTime(dateTimeOffset, VietnamTimeZone);
        }

        /// <summary>
        /// Chuyển đổi và định dạng chuỗi hiển thị theo giờ Việt Nam
        /// </summary>
        /// <param name="format">Định dạng mong muốn, mặc định là dd/MM/yyyy HH:mm</param>
        public static string ToVietnamString(this DateTimeOffset dateTimeOffset, string format = "dd/MM/yyyy HH:mm")
        {
            return dateTimeOffset.ToVietnamTime().ToString(format);
        }

        /// <summary>
        /// Tiện ích lấy riêng giờ và phút (VD: 14:30)
        /// </summary>
        public static string ToVietnamTimeString(this DateTimeOffset dateTimeOffset)
        {
            return dateTimeOffset.ToVietnamTime().ToString("HH:mm");
        }

        /// <summary>
        /// từ giờ utc sang DateOnly theo giờ Việt Nam (giữ nguyên ngày tháng năm theo giờ VN)
        /// </summary>
        public static DateOnly UtcToVietnamDateOnly(this DateTimeOffset dateTimeOffset)
        {
            var vietnamTime = TimeZoneInfo.ConvertTime(dateTimeOffset, VietnamTimeZone);
            return DateOnly.FromDateTime(vietnamTime.DateTime);
        }

        /// <summary>
        /// từ giờ utc sang TimeOnly theo giờ Việt Nam (giữ nguyên giờ phút theo giờ VN)
        /// </summary>
        public static TimeOnly ToVietnamTimeOnly(this DateTimeOffset dateTimeOffset)
        {
            var vietnamTime = TimeZoneInfo.ConvertTime(dateTimeOffset, VietnamTimeZone);
            return TimeOnly.FromDateTime(vietnamTime.DateTime);
        }

        /// <summary>
        /// 
        /// </summary>
        public static DateOnly StringToVietnamDateOnly(this string date)
        {
            return DateOnly.ParseExact(date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }

        public static bool TryStringToVietnamDateOnly(this string? date, out DateOnly result)
        {
            return DateOnly.TryParseExact(
                date,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out result
            );
        }

        /// <summary>
        /// Chuyển từ giờ Việt Nam (UTC+7) sang UTC
        /// </summary>
        public static DateTimeOffset VietnamToUtc(this DateTimeOffset vietnamTime)
        {
            return vietnamTime.ToUniversalTime();
        }

        /// <summary>
        /// Từ string ngày (dd/MM/yyyy) + TimeOnly (giờ VN) -> DateTimeOffset UTC
        /// </summary>
        public static DateTimeOffset ToUtcDateTimeOffset(this string date, TimeOnly time)
        {
            // 1. Parse ngày VN
            var dateOnly = date.StringToVietnamDateOnly();

            // 2. Ghép Date + Time → DateTime (KHÔNG có timezone)
            var localDateTime = dateOnly.ToDateTime(time);

            // 3. Gán timezone VN (+7)
            var vietnamDateTimeOffset = new DateTimeOffset(
                localDateTime,
                VietnamTimeZone.GetUtcOffset(localDateTime)
            );

            // 4. Convert sang UTC
            return vietnamDateTimeOffset.ToUniversalTime();
        }

        /// <summary>
        /// Chuyển string "HH:mm" (giờ VN) sang TimeOnly
        /// </summary>
        public static TimeOnly StringToVietnamTimeOnly(this string time)
        {
            return TimeOnly.ParseExact(time, "HH:mm", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// check xem string có phải định dạng giờ Việt Nam hợp lệ hay không (HH:mm)
        /// </summary>
        public static bool IsValidVietnamTime(this string? time)
        {
            return TimeOnly.TryParseExact(
                time?.Trim(),
                "HH:mm",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _
            );
        }
    }
}