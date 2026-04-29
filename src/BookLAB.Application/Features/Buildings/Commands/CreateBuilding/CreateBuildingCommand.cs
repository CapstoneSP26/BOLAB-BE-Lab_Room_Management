using BookLAB.Application.Features.Buildings.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Buildings.Commands.CreateBuilding
{
    public class CreateBuildingCommand : IRequest<BuildingDto>
    {
        public int CampusId { get; set; }
        public string BuildingName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? BuildingImageUrl { get; set; }
    }
}
