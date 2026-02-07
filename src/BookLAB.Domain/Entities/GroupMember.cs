using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class GroupMember : BaseEntity
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
        public virtual Group Group { get; set; }
        public virtual User User { get; set; }
    }
}
