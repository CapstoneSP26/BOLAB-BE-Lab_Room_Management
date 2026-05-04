using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Report : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid ScheduleId { get; set; }
        public int? ReportTypeId { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsResolved { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public virtual Schedule Schedule { get; set; }
        public virtual ReportType? ReportType { get; set; }
        public ICollection<ReportImage> ReportImages = new List<ReportImage>();

    }
}
