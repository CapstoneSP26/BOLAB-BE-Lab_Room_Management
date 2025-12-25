using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class Booking : BaseEntity, ISoftDeletable, IAuditable, IUserTrackable
    {
        public Guid RoomId { get; set; }
        public Guid SemesterId { get; set; }
        public Guid BookedByUserId { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Guid SlotTypeId { get; set; }
        public int? ExpectedParticipantCount { get; set; }
        public BookingParticipantMode ParticipantMode { get; set; }

        public string Purpose { get; set; } = null!;
        public BookingStatus Status { get; set; }

        public bool IsCourseSchedule { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public ICollection<BookingGroup> BookingGroups { get; set; } = new List<BookingGroup>();
        public ICollection<BookingUser> BookingUsers { get; set; } = new List<BookingUser>();
    }
}
