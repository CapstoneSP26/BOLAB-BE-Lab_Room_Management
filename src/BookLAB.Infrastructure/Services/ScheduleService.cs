using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
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

        public RowResult<ScheduleImportDto> CheckSingleRowAsync(
            ScheduleImportDto item,
            Dictionary<string, LabRoom> roomMap,
            Dictionary<string, User> lecturerMap,
            Dictionary<string, Group> groupMap,
            Dictionary<string, List<SlotFrame>> slotTypeMap,
            CancellationToken ct)
        {
            var rowResult = new RowResult<ScheduleImportDto> { Data = item };
            var normalizedRoomCode = item.RoomNo.Trim().TrimEnd('.');

            // --- A. Check SlotTypeCode & SlotOrder ---
            SlotFrame? targetFrame = null;
            if (!slotTypeMap.TryGetValue(item.SlotTypeCode, out var frames))
            {
                rowResult.Messages.Add($"Mã hệ đào tạo '{item.SlotTypeCode}' không tồn tại.");
                rowResult.Status = "Invalid";
                rowResult.IsCritical = true;
            }
            else
            {
                targetFrame = frames.FirstOrDefault(f => f.OrderIndex == item.SlotOrder);
                if (targetFrame == null)
                {
                    rowResult.Messages.Add($"Slot {item.SlotOrder} không có định nghĩa khung giờ trong hệ {item.SlotTypeCode}.");
                    rowResult.Status = "Invalid";
                    rowResult.IsCritical = true;
                }
            }

            // --- B. Check LabRoom ---
            if (!roomMap.TryGetValue(normalizedRoomCode, out var room))
            {
                rowResult.Messages.Add($"Phòng '{item.RoomNo}' không tồn tại trên hệ thống.");
                rowResult.Status = "Invalid";
                rowResult.IsCritical = true;
            }

            // --- C. Check Lecturer ---
            if (!lecturerMap.ContainsKey(item.Lecturer))
            {
                rowResult.Messages.Add($"Cảnh báo: Không tìm thấy giảng viên '{item.Lecturer}'. Cần kiểm tra lại mã nhân viên.");
                if (rowResult.Status != "Invalid") rowResult.Status = "Warning";
            }

            // --- D. Check GroupName ---
            if (!groupMap.ContainsKey(item.GroupName))
            {
                rowResult.Messages.Add($"Không tìm thấy tên group");
                rowResult.Status = "Invalid";
                rowResult.IsCritical = true;
            }

            // --- E. Check DateTime Format ---
            if (!DateTime.TryParseExact(item.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var sessionDate))
            {
                rowResult.Messages.Add("Ngày học không hợp lệ (yêu cầu dd/MM/yyyy).");
                rowResult.Status = "Invalid";
                rowResult.IsCritical = true;
            }

            return rowResult;
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
