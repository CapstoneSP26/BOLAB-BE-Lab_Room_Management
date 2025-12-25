using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class User : BaseEntity, ISoftDeletable
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? StudentId { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
