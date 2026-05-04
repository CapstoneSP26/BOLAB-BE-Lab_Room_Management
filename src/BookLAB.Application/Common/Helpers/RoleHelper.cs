
namespace BookLAB.Application.Common.Helpers
{
    public static class RoleHelper
    {
        public static List<string> ParseRoles(string? roleNames)
        {
            return (roleNames ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();
        }
    }
}
