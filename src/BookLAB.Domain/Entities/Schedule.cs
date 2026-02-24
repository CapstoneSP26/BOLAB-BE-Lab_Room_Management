using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class Schedule : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid LecturerId {  get; set; }
        public int LabRoomId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? GroupId { get; set; }
        public int SlotTypeId { get; set; }
        public ScheduleType ScheduleType { get; set; }
        public ScheduleStatus ScheduleStatus { get; set; }
        public int StudentCount { get; set; }
        public string? SubjectCode { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        public virtual User User { get; set; } = new User();
        public virtual LabRoom LabRoom { get; set; } = new LabRoom();
        public virtual Booking? Booking { get; set; }
        public virtual Group? Group { get; set; }
        public virtual SlotType SlotType { get; set; } = new SlotType();
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
