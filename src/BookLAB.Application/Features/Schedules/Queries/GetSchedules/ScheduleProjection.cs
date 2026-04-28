using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Schedules.Queries.GetSchedules
{
    public static class ScheduleProjection
    {
        public static IQueryable<ScheduleDto> SelectSchedule(this IQueryable<Schedule> query)
        {
            return query.Select(x => new ScheduleDto
            {
                Id = x.Id,
                SubjectCode = x.SubjectCode,
                LecturerName = x.User.FullName,
                UserCode = x.User.UserCode,
                LabRoomName = x.LabRoom.RoomName,
                SlotName = x.SlotType.Name,
                StartTime = x.StartTime,
                EndTime = x.EndTime,
                GroupName = x.Group.GroupName,
                RoomNo = x.LabRoom.RoomNo,
                StudentCount = x.StudentCount,         
                Status = x.ScheduleStatus,
                Type = x.ScheduleType,
                CreatedAt = x.CreatedAt,
            });
        }
    }
}
