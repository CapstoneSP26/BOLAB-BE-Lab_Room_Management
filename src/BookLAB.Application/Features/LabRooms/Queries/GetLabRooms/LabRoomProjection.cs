using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;

public static class LabRoomProjection
{
    public static IQueryable<LabRoomDto> SelectLabRoom(
        this IQueryable<LabRoom> query,
        bool includeImages,
        bool includeBuilding)
    {
        return query.Select(x => new LabRoomDto
        {
            Id = x.Id,
            RoomNo = x.RoomNo,
            RoomName = x.RoomName,
            Capacity = x.Capacity,
            HasEquipment = x.HasEquipment,
            Description = x.Description,
            BuildingId = x.BuildingId,
            Location = x.Location,
            IsActive = x.IsActive,

            // 🔥 Control JOIN
            BuildingName = includeBuilding
                ? x.Building.BuildingName
                : null,

            Images = includeImages
                ? x.LabImages.Select(i => new LabImageDto
                {
                    Id = i.Id,
                    Url = i.ImageUrl,
                    IsPrimary = i.IsAvatar
                }).ToList()
                : null
        });
    }
}