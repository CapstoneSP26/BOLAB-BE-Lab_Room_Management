using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Group: BaseEntity, ISoftDeletable, IAuditable, IUserTrackable
    {
        public string GroupName { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public int? CampusId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public virtual Campus Campus { get; set; }
        public virtual User User { get; set; }
    }
}
