using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;
using System.ComponentModel;

namespace BookLAB.Domain.Entities
{
    public class Report : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid ScheduleId { get; set; }
        public ReportType ReportType { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsResolved { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public virtual Schedule Schedule { get; set; } = new Schedule();
    }
}
