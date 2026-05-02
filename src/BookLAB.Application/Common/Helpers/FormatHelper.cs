namespace BookLAB.Application.Common.Helpers
{
    public static class FormatHelper
    {
        public static string Normalize(string? value)
    => string.IsNullOrWhiteSpace(value)
        ? string.Empty
        : value.Trim().ToUpperInvariant();
    }
}
