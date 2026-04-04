using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using System.Globalization;

namespace BookLAB.Infrastructure.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleService(IUnitOfWork unitOfWork,
            IScheduleRepository scheduleRepository)
        {
            _unitOfWork = unitOfWork;
            _scheduleRepository = scheduleRepository;
        }

        public async Task<bool> AddScheduleAsync(Schedule schedule)
        {
            return await _scheduleRepository.AddScheduleAsync(schedule);
        }

        public async Task<bool> CheckConflictAsync(int roomId, DateTimeOffset startTime, DateTimeOffset endTime, CancellationToken cancellationToken)
        {
            if (startTime > endTime) return true;

            var conflicts = await _scheduleRepository.CheckConflictAsync(roomId, startTime, endTime, cancellationToken);

            return conflicts;
        }

        public Schedule ConvertToScheduleEntity(
            ScheduleImportDto item,
            Dictionary<string, LabRoom> roomMap,
            Dictionary<string, User> lecturerMap,
            Dictionary<string, Group> groupMap,
            Dictionary<string, List<SlotFrame>> slotTypeMap,
            CancellationToken ct)   
        {
            var normalizedRoomCode = item.RoomNo.Trim().TrimEnd('.');
            var room = roomMap[normalizedRoomCode];
            var group = groupMap[item.GroupName];
            var lecturer = lecturerMap[item.Lecturer];
            var slotFrames = slotTypeMap[item.SlotTypeCode];
            var targetFrame = slotFrames.First(f => f.OrderIndex == item.SlotOrder);
            var sessionDate = DateTime.ParseExact(item.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            var start = new DateTimeOffset(
                    sessionDate.Year,
                    sessionDate.Month,
                    sessionDate.Day,
                    targetFrame.StartTimeSlot.Hour,
                    targetFrame.StartTimeSlot.Minute,
                    targetFrame.StartTimeSlot.Second,
                    TimeSpan.FromHours(7));
            var end = new DateTimeOffset(
                    sessionDate.Year,
                    sessionDate.Month,
                    sessionDate.Day,
                    targetFrame.EndTimeSlot.Hour,
                    targetFrame.EndTimeSlot.Minute,
                    targetFrame.EndTimeSlot.Second,
                    TimeSpan.FromHours(7));
            return new Schedule
            {
                GroupId = group.Id,
                SubjectCode = item.SubjectCode,
                LecturerId = lecturer.Id,
                LabRoomId = room.Id,
                StartTime = start,
                EndTime = end,
                ScheduleType = ScheduleType.Academic,
                ScheduleStatus = ScheduleStatus.Active,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
