using AutoMapper;
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
        }
    }
}
