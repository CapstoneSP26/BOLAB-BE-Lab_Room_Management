namespace BookLAB.Application.Features.Bookings.Queries.GetPurposeTypes
{
    public record PurposeTypeDto
    {
        public int Id { get; init; }
        public string PurposeName { get; init; } = string.Empty;
    }
}
