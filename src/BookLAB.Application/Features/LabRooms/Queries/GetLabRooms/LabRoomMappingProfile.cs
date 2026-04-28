using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;

public class LabRoomMappingProfile : AutoMapper.Profile
{
    public LabRoomMappingProfile()
    {
        CreateMap<LabRoom, LabRoomDto>()
            .ForMember(d => d.BuildingId, opt => opt.MapFrom(s => s.BuildingId))
            .ForMember(d => d.BuildingName, opt => opt.MapFrom(s => s.Building.BuildingName))
            .ForMember(d => d.RoomNo, opt => opt.MapFrom(s => s.RoomNo))
            .ForMember(d => d.RoomName, opt => opt.MapFrom(s => s.RoomName))
            .ForMember(d => d.Capacity, opt => opt.MapFrom(s => s.Capacity))
            .ForMember(d => d.HasEquipment, opt => opt.MapFrom(s => s.HasEquipment))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description))
            .ForMember(d => d.Images, opt => opt.MapFrom(s => (s.LabImages != null && s.LabImages.Any()) ? s.LabImages : null));

        CreateMap<LabImage, LabImageDto>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            .ForMember(d => d.Url, opt => opt.MapFrom(s => s.ImageUrl))
            .ForMember(d => d.IsPrimary, opt => opt.MapFrom(s => s.IsAvatar));
    }
}