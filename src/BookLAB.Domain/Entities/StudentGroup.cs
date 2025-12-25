using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class StudentGroup : BaseEntity, ISoftDeletable, IAuditable, IUserTrackable
    {
        public string GroupName { get; set; } = null!;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}