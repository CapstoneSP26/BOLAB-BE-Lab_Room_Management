using AutoMapper;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Buildings.Queries.GetBuildingsInCampus;
using BookLAB.Application.Features.LabRooms.Queries;
using BookLAB.Application.Features.Schedules.Queries.GetAvailableSlots;
using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace BookLAB.Application.Common.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Building, BuildingDto>().ReverseMap();
            CreateMap<Building, BuildingResponse>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.BuildingName))
                .ReverseMap();
            CreateMap<LabRoom, LabRoomRequest>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.name, opt => opt.MapFrom(src => src.RoomName))
                .ForMember(dest => dest.building, opt => opt.MapFrom(src => src.BuildingId))
                .ForMember(dest => dest.capacity, opt => opt.MapFrom(src => src.Capacity))
                .ForMember(dest => dest.image, opt => opt.MapFrom(src => src.LabImages
                    .Where(i => i.LabRoomId == src.Id && i.IsAvatar == true)
                    .ToList().FirstOrDefault().ImageUrl))
                .ReverseMap();
            CreateMap<Schedule, AvailableScheduleResponse>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.roomId, opt => opt.MapFrom(src => src.LabRoomId.ToString()))
                .ForMember(dest => dest.date, opt => opt.MapFrom(src => src.StartTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.startTime, opt => opt.MapFrom(src => src.StartTime.ToString("HH-mm", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.endTime, opt => opt.MapFrom(src => src.EndTime.ToString("HH-mm", CultureInfo.InvariantCulture)))
                .ForMember(dest => dest.status, opt => opt.MapFrom(src => src.ScheduleStatus))
                .ForMember(dest => dest.bookedBy, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.groupName, opt => opt.MapFrom(src => src.Group.GroupName))
                //.ForMember(dest => dest.slotType, opt => opt.MapFrom(src => src.SlotType.Name))
                //.ForMember(dest => dest.slotNumber, opt => opt.MapFrom(src => src.ScheduleStatus))
                .ReverseMap();
        }
    }
}
