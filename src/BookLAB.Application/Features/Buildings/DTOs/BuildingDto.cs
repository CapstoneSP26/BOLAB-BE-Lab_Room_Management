namespace BookLAB.Application.Features.Buildings.DTOs
{
    public record BuildingDto
    {
        public int Id { get; init; }
        public int CampusId { get; init; }
        public int BuildingId { get; init; }
        public string BuildingName { get; init; } = null!;
        public string Description { get; init; } = null!;
        public string BuildingImageUrl { get; init; } = null!;
        public string? CampusName { get; init; } // Phẳng hóa dữ liệu từ Campus
    }
}
