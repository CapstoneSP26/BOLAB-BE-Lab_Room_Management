namespace BookLAB.Domain.Enums
{
    public enum PolicyType
    {
        IsFreeTimeAllowed = 1,          // Cho phép đặt giờ tự do (OutSlot)
        MinBookingLeadTime = 2,         // Thời gian đặt trước tối thiểu (Giờ)
        MaxBookingAdvance = 3,          // Thời gian đặt trước tối đa (Ngày)
        CurfewTime = 4,                 // Giờ giới nghiêm (VD: 22:00)
        MaxOutSlotDuration = 6,         // Thời lượng tối đa cho 1 lần đặt tự do
        MaxConcurrentBookings = 7       // Đây là tên mới cho OverrideNumber
    }
}
