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
    }
}