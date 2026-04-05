namespace BookLAB.Application.Features.Users.Common
{
    public class UserImportDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public string RoleNames { get; set; } = string.Empty;
        public string CampusCode { get; set; } = string.Empty;
        public bool IsUpdated { get; set; } = false;
        public bool IsValid { get; set; } = true;
    }
}
