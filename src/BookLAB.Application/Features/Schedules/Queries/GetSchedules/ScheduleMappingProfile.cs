using AutoMapper;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Schedules.Queries.GetSchedules;

public class ScheduleMappingProfile : AutoMapper.Profile
{
    public ScheduleMappingProfile()
    {
        CreateMap<Schedule, ScheduleDto>()
            // 1. Map thông tin định danh cơ bản
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.Id))
            // 2. Lấy tên Giảng viên từ bảng User (Eager Loading từ Include)
            .ForMember(d => d.LecturerName, opt => opt.MapFrom(s => s.User.FullName))

            // 3. Lấy tên phòng Lab và mã môn học
            .ForMember(d => d.LabRoomName, opt => opt.MapFrom(s => s.LabRoom.RoomName))
            .ForMember(d => d.SubjectCode, opt => opt.MapFrom(s => s.SubjectCode))

            // 4. Lấy tên Group (nếu có)
            .ForMember(d => d.GroupName, opt => opt.MapFrom(s => s.Group != null ? s.Group.GroupName : "N/A"))

            // 5. Chuyển đổi Enum sang String để Frontend dễ hiển thị
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.ScheduleStatus.ToString()))
            .ForMember(d => d.Type, opt => opt.MapFrom(s => s.ScheduleType.ToString()))

            // 6. Thông tin thời gian và số lượng
            .ForMember(d => d.StartTime, opt => opt.MapFrom(s => s.StartTime))
            .ForMember(d => d.EndTime, opt => opt.MapFrom(s => s.EndTime))
            .ForMember(d => d.StudentCount, opt => opt.MapFrom(s => s.StudentCount));
    }
}