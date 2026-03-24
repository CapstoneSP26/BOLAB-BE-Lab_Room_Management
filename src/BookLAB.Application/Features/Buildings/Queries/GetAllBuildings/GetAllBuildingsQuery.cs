using MediatR;
using BookLAB.Application.Features.Buildings.Queries.GetBuildingByName;

namespace BookLAB.Application.Features.Buildings.Queries.GetAllBuildings
{
    public class GetAllBuildingsQuery : IRequest<List<BuildingDto>>
    {
    }
}