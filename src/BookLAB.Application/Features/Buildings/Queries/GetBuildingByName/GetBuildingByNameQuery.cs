using BookLAB.Application.Features.Buildings.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Buildings.Queries.GetBuildingByName
{
    public class GetBuildingByNameQuery : IRequest<BuildingDto?>
    {
        public string BuildingName { get; set; } = null!;
    }
}
