using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class Attendance : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset? CheckInTime { get; set; }
        public DateTimeOffset? CheckOutTime { get; set; }
        public AttendanceCheckInMethod CheckInMethod { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public virtual Booking Booking { get; set; }
        public virtual User User { get; set; }

    }
}
