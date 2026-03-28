using AutoMapper;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Groups.DTOs;
using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Mappings
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Booking, BookingDto>()
                .ReverseMap();

            // Group mappings
            CreateMap<Group, GroupDto>()
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.MembersCount, opt => opt.Ignore()); // Handled separately in handler

            CreateMap<Report, ReportDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.ReportType, opt => opt.MapFrom(src => src.ReportType.ReportTypeName))
                .ForMember(dest => dest.LabRoomId, opt => opt.MapFrom(src => src.Schedule.LabRoomId))
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Schedule.LabRoom.RoomName))
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Schedule.LabRoom.Building.BuildingName))
                .ForMember(dest => dest.Reason, opt => opt.MapFrom(src => src.ReportType.ReportTypeName));
        }
    }
}
