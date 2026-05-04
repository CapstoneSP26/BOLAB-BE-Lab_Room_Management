namespace BookLAB.Domain.Enums
{
    public enum ScheduleType
    {
        Academic = 1,    // Lịch chính thức từ phòng Đào tạo (Import file)
        Personal = 2,    // Lịch đặt cá nhân của Giảng viên (Booking)
        Maintenance = 3, // Lịch bảo trì thiết bị (Phòng Lab đóng cửa để sửa chữa)
        Examination = 4, // Lịch thi (Cần ưu tiên cao nhất, không được đụng vào)
        Event = 5        // Lịch hội thảo, workshop hoặc sự kiện đột xuất
    }
}
