using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;
using System.Globalization;

namespace BookLAB.Infrastructure.Services
{
    public class ScheduleImportService : IScheduleImportService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleImportService(IUnitOfWork unitOfWork,
            IScheduleRepository scheduleRepository)
        {
            _unitOfWork = unitOfWork;
            _scheduleRepository = scheduleRepository;
        }

        public RowResult<ScheduleImportDto> CheckSingleRowAsync(
            List<ScheduleImportDto> scheduleImportList,
            ScheduleImportDto item,
            Dictionary<string, LabRoom> roomMap,
            Dictionary<string, User> lecturerMap,
            Dictionary<string, Group> groupMap,
            Dictionary<string, List<SlotFrame>> slotTypeMap,
            List<string> existingHashes,
            CancellationToken ct)
        {
            var rowResult = new RowResult<ScheduleImportDto> { Data = item };
            var normalizedRoomCode = item.RoomNo.Trim().TrimEnd('.');

            // --- A. Check SlotTypeCode & SlotOrder ---
            SlotFrame? targetFrame = null;
            if (!slotTypeMap.TryGetValue(item.SlotTypeCode, out var frames))
            {
                rowResult.Errors.Add(new RowError
                {
                    FieldName = "TypeSlot",
                    Message = "Mã slot không tồn tại",
                    Severity = ErrorSeverity.Error
                });
            }
            else
            {
                targetFrame = frames.FirstOrDefault(f => f.OrderIndex == item.SlotOrder);
                if (targetFrame == null)
                {
                    rowResult.Errors.Add(new RowError
                    {
                        FieldName = "Slot",
                        Message = "Slot Order không tồn tại",
                        Severity = ErrorSeverity.Error
                    });
                }
            }

            // --- B. Check LabRoom ---
            if (!roomMap.TryGetValue(normalizedRoomCode, out var room))
            {
                rowResult.Errors.Add(new RowError
                {
                    FieldName = "RoomNo",
                    Message = "Phòng không tồn tại trên hệ thống",
                    Severity = ErrorSeverity.Error
                });
            }

            // --- C. Check Lecturer ---
            if (!lecturerMap.ContainsKey(item.Lecturer))
            {
                rowResult.Errors.Add(new RowError
                {
                    FieldName = "Lecturer",
                    Message = "Giảng viên không tồn tại trên hệ thống",
                    Severity = ErrorSeverity.Error
                });
            }

            // --- D. Check GroupName ---
            if (!groupMap.ContainsKey(item.GroupName))
            {
                rowResult.Errors.Add(new RowError
                {
                    FieldName = "GroupName",
                    Message = "Tên nhóm không tồn tại trên hệ thống",
                    Severity = ErrorSeverity.Error
                });
            }

            // --- E. Check DateTime Format ---
            if (!DateTime.TryParseExact(item.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var sessionDate))
            {
                rowResult.Errors.Add(new RowError
                {
                    FieldName = "Date",
                    Message = "Ngày học không hợp lệ",
                    Severity = ErrorSeverity.Error
                });
            }

            // --- BƯỚC 2: CHECK IDEMPOTENCY (Import trùng) ---
            var currentHash = GenerateHash(item);
            if (existingHashes.Contains(currentHash))
            {
                rowResult.Errors.Add(new RowError
                {
                    FieldName = "Duplicate",
                    Message = "Lịch này đã tồn tại trong hệ thống",
                    Severity = ErrorSeverity.Warning
                });
            }


            return rowResult;
        }

        private string GenerateHash(ScheduleImportDto d)
        => $"{d.GroupName}_{d.Date}_{d.SlotOrder}_{d.RoomNo}_{d.SlotTypeCode}";

    }
}
