using BookLAB.Application.Features.Buildings.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Buildings.Queries.GetAllBuildings
{
    public class GetAllBuildingsQuery : IRequest<List<BuildingDto>>
    {
    }
}