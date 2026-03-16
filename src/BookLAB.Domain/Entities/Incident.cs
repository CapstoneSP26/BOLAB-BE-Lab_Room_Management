using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class Incident : BaseEntity, IAuditable, IUserTrackable
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IncidentSeverity Severity { get; set; }
        public string? Environment { get; set; }
        public List<string> StepsToReproduce { get; set; } = new();
        public string? ExpectedResult { get; set; }
        public string? ActualResult { get; set; }
        public string? AttachmentUrl { get; set; }
        public Guid ReportedBy { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public User ReportedByUser { get; set; } = null!;
    }
}