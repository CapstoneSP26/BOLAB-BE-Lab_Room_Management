using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class AttendanceDetail : BaseEntity, IAuditable
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }

        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        public AttendanceCheckInMethod CheckInMethod { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
