using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class AttendanceSummary : BaseEntity, IAuditable
    {
        public Guid BookingId { get; set; }

        public AttendanceMode AttendanceMode { get; set; }

        public int TotalParticipants { get; set; }
        public int? TotalGroups { get; set; }

        public Guid RecordedByUserId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
