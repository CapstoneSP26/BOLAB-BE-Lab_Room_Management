namespace BookLAB.Application.Features.Users.Queries.GetUserProfile
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public string CampusName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
