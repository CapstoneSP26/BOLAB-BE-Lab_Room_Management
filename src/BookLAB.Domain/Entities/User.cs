using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class User : BaseEntity, ISoftDeletable, IAuditable, IUserTrackable
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public string UserImageUrl { get; set; } = string.Empty;
        public string? Provider { get; set; } 
        public string? ProviderId { get; set; }
        public int CampusId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; } = true;
        public Campus? Campus { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}
