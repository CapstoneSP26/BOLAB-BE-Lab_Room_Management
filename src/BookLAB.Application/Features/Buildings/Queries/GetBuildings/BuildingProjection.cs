using BookLAB.Application.Features.Buildings.DTOs;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Buildings.Queries.GetBuildings
{
    public static class BuildingProjection
    {
        public static IQueryable<BuildingDto> SelectBuilding(
            this IQueryable<Building> query,
            IQueryable<LabRoom> labRoomQuery)
        {
            return query.Select(x => new BuildingDto
            {
                Id = x.Id,
                CampusId = x.CampusId,
                BuildingId = x.Id,
                BuildingName = x.BuildingName,
                Description = x.Description,
                BuildingImageUrl = x.BuildingImageUrl,
                RoomCount = labRoomQuery.Count(r => r.BuildingId == x.Id),
                CampusName = x.Campus.CampusName
            });
        }
    }
}
