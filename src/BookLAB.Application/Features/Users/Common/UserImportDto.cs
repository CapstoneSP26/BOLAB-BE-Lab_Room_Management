namespace BookLAB.Application.Features.Users.Common
{
    public class UserImportDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public int CampusId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
