namespace BookLAB.Application.Common.Models
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string>? Roles { get; set; } = new();
        public List<int>? RoleIds { get; set; } = new();
        public int CampusId { get; set; }
        public string? AvatarUrl { get; set; }
        public string? UserCode { get; set; }
        public bool? IsActive { get; set; }
    }
}
