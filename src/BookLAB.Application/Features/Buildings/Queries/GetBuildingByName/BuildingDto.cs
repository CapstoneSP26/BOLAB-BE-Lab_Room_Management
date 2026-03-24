namespace BookLAB.Application.Features.Buildings.Queries.GetBuildingByName
{
    public class BuildingDto
    {
        public int Id { get; set; }
        public int CampusId { get; set; }
        public string BuildingName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string BuildingImageUrl { get; set; } = null!;
        public string? CampusName { get; set; }
    }
}
