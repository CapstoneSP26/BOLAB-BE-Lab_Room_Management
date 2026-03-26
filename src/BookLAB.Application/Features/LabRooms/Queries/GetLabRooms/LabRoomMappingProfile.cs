using AutoMapper;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.LabRooms.Queries.GetLabRooms;

public class LabRoomMappingProfile : Profile
{
    public LabRoomMappingProfile()
    {
        CreateMap<LabRoom, LabRoomDto>()
            .ForMember(d => d.BuildingName, opt => opt.MapFrom(s => s.Building.BuildingName))
            .ForMember(d => d.RoomNo, opt => opt.MapFrom(s => s.RoomNo))
            .ForMember(d => d.RoomName, opt => opt.MapFrom(s => s.RoomName))
            .ForMember(d => d.Capacity, opt => opt.MapFrom(s => s.Capacity))
            .ForMember(d => d.HasEquipment, opt => opt.MapFrom(s => s.HasEquipment))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description));
    }
}