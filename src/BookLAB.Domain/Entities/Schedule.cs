using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Schedule : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid LecturerId {  get; set; }
        public int LabRoomId { get; set; }
        public string ScheduleType { get; set; } = string.Empty;
        public string ScheduleStatus { get; set; } = string.Empty;
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
        public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
