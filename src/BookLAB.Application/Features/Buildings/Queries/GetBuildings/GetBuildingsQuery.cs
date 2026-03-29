using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Buildings.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Buildings.Queries.GetBuildings
{
    public class GetBuildingsQuery : IRequest<PagedList<BuildingDto>>
    {
        public int? CampusId { get; set; }
        public string? BuildingName { get; set; }
        public string? SearchTerm { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 0;
    }
}
