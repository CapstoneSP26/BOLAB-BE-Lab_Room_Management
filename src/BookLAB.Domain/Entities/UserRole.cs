using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class UserRole : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
        public virtual User User { get; set; } = new User();
        public virtual Role Role { get; set; } = new Role();
    }
}
