using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using AutoMapper;

namespace BookLAB.Application.Features.Users
{
    public class UserProfileMappingProfile : Profile
    {
        public UserProfileMappingProfile()
        {
            CreateMap<User, UserProfileDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.UserCode))
                .ForMember(dest => dest.UserImageUrl, opt => opt.MapFrom(src => src.UserImageUrl))
                .ForMember(dest => dest.CampusId, opt => opt.MapFrom(src => src.CampusId))
                .ForMember(dest => dest.CampusName, opt => opt.MapFrom(src => src.Campus != null ? src.Campus.CampusName : ""))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.DateTime))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UserRoles != null ? src.UserRoles.Select(ur => ur.Role.RoleName).ToList() : new List<string>()));
        }
    }
}
