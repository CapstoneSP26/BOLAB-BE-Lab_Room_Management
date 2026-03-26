namespace BookLAB.Application.Common.Models
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public string UserImageUrl { get; set; } = string.Empty;
        public int CampusId { get; set; }
        public string CampusName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class UpdateUserProfileRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string UserImageUrl { get; set; } = string.Empty;
    }
}
