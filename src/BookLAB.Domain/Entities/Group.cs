using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Group: BaseEntity, ISoftDeletable, IAuditable, IUserTrackable
    {
        public string GroupName { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public virtual User User { get; set; } = new User();
    }
}
