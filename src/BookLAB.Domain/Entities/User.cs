using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class User : BaseEntity, ISoftDeletable, IAuditable, IUserTrackable
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserImageUrl { get; set; } = string.Empty;
        public Guid CampusId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; } = true;
        public Campus Campus { get; set; } = new Campus();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
