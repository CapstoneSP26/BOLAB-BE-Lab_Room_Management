
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Users.Common
{
    public class UserImportMaps
    {
        public HashSet<string> ExistingEmails { get; set; } = new();
        public HashSet<string> ExistingCodes { get; set; } = new();
        public Dictionary<string, Role> RoleMap { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public Dictionary<string, Campus> CampusMap { get; set; } = new();
        public Dictionary<string, User> UserMap { get; set; } = new();
        public HashSet<string> SeenEmails { get; set; } = new(StringComparer.OrdinalIgnoreCase);
        public HashSet<string> SeenCodes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    }
}
