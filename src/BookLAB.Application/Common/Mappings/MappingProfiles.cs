using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Queries.ViewUncheckedBookingRequest;
using BookLAB.Application.Features.Groups.DTOs;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Mappings
{
    public class MappingProfiles : AutoMapper.Profile
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

            CreateMap<BookingRequest, BookingRequestFe>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Booking.Id))
                .ForMember(dest => dest.RoomId, opt => opt.MapFrom(src => src.Booking.LabRoomId))
                .ForMember(dest => dest.RoomName, opt => opt.MapFrom(src => src.Booking.LabRoom.RoomName))
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Booking.LabRoom.Building.BuildingName))
                .ForMember(dest => dest.RequestedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.RequestedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.Booking.StartTime.ToString("HH:mm")))
                .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.Booking.EndTime.ToString("HH:mm")))
                .ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Booking.StartTime.ToString("dd-MM-yyyy")))
                .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Booking.StudentCount))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.BookingRequestStatus))
                .ForMember(dest => dest.Purpose, opt => opt.MapFrom(src => src.Booking.PurposeType.PurposeName))
                .ForMember(dest => dest.Requester, opt => opt.MapFrom(src => src.Requester));

            CreateMap<Schedule, ScheduleDto2>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.User.UserCode))
                .ForMember(dest => dest.LabRoomName, opt => opt.MapFrom(src => src.LabRoom.RoomName))
                .ForMember(dest => dest.SlotName, opt => opt.MapFrom(src => src.SlotType != null ? src.SlotType.Name : "Flexible"))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.ScheduleStatus))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ScheduleType))
                .ReverseMap();

            CreateMap<Schedule, ScheduleDto>();

            CreateMap<Schedule, BookLAB.Application.Features.Schedules.Queries.GetSchedules.ScheduleDto>()
                .ForMember(dest => dest.RoomNo, opt => opt.MapFrom(src => src.LabRoom.RoomNo))
                .ForMember(dest => dest.LecturerName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.UserCode, opt => opt.MapFrom(src => src.User.UserCode))
                .ForMember(dest => dest.LabRoomName, opt => opt.MapFrom(src => src.LabRoom.RoomName))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.GroupName));


            CreateMap<User, UserProfileDto>()
                .ForMember(dest => (string)dest.AvatarUrl, opt => opt.MapFrom(src => src.UserImageUrl))
                .ForMember(dest => (string)dest.UserCode, opt => opt.MapFrom(src => src.UserCode))
                .ForMember(dest => (bool)dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => (List<string>)dest.Roles, opt => opt.MapFrom(src => src.UserRoles.Select(x => x.Role.RoleName).ToList()))
                .ForMember(dest => (List<int>)dest.RoleIds, opt => opt.MapFrom(src => src.UserRoles.Select(x => x.RoleId).ToList()));
        }
    }
}
