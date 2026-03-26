using BookLAB.Application.Features.Buildings.DTOs;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Buildings.Queries.GetBuildings
{
    public static class BuildingProjection
    {
        public static IQueryable<BuildingDto> SelectBuilding(
            this IQueryable<Building> query)
        {
            return query.Select(x => new BuildingDto
            {
                Id = x.Id,
                CampusId = x.CampusId,
                BuildingName = x.BuildingName,
                Description = x.Description,
                BuildingImageUrl = x.BuildingImageUrl,
                CampusName = x.Campus.CampusName 
            });
        }
    }
}
