using BookLAB.Application.Features.Buildings.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Buildings.Commands.UpdateBuilding
{
    public class UpdateBuildingCommand : IRequest<BuildingDto>
    {
        public int Id { get; set; }
        public int CampusId { get; set; }
        public string BuildingName { get; set; } = null!;
        public string? Description { get; set; }
        public string? BuildingImageUrl { get; set; }
    }
}
