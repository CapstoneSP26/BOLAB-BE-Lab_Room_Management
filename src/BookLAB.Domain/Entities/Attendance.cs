using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class Attendance : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public AttendanceCheckInMethod CheckInMethod { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public virtual Booking Booking { get; set; } = new Booking();
        public virtual User User { get; set; } = new User();

    }
}
