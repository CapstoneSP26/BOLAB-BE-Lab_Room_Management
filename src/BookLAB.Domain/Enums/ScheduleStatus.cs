namespace BookLAB.Domain.Enums
{
    public enum ScheduleStatus
    {
        // 1. Lịch đã sẵn sàng, đang chờ đến giờ bắt đầu
        Active = 1,

        // 2. Giảng viên đã có mặt/Check-in và đang sử dụng phòng
        InProcess = 2,

        // 3. Buổi học kết thúc bình thường
        Completed = 3,

        // 4. Lịch bị hủy (do giảng viên hủy hoặc bị Import đè)
        Cancelled = 4,
    }
}
