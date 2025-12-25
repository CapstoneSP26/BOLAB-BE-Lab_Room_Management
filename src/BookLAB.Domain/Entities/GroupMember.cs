using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class GroupMember : BaseEntity
    {
        public Guid GroupId { get; set; }
        public Guid UserId { get; set; }
    }
}
