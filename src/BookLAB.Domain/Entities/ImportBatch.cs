using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class ImportBatch : BaseEntity, ISoftDeletable, IAuditable, IUserTrackable
    {
        public string Name { get; set; } = string.Empty;
        public ImportBatchType ImportBatchType { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
