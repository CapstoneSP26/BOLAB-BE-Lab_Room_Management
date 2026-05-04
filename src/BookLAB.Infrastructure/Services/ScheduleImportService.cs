using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Helpers;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BookLAB.Infrastructure.Services
{
    public class ScheduleImportService : IScheduleImportService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ScheduleImportService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ScheduleImportValidateResponse> ValidateAsync(
        List<ScheduleImportDto> schedules,
        int campusId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        Guid? importBatchId,
        CancellationToken ct,
        bool isAllowCreateImportData = false)
        {
            // 1. Init
            var response = InitResponse(schedules);

            // 2. Prefetch
            var maps = await BuildMapsAsync(schedules, campusId, startTime, endTime, importBatchId, ct);

            // 3. Row validation
            ValidateRows(schedules, maps, startTime, endTime, response);

            // 4. Duplicate
            CheckDuplicate(schedules, maps, response);

            // 5. Build conflict data
            var conflictData = BuildConflictData(schedules, importBatchId, maps);

            // 6. Lecturer conflict
            CheckLecturerConflict(conflictData, response);

            // 7. Room conflict
            CheckRoomConflict(conflictData, maps, response);
            if (isAllowCreateImportData && response.CanCommit)
            {
                // 8. Convert to Schedule 
                ConvertToScheduleEntites(maps, response);
            }
            return new ScheduleImportValidateResponse
            {
                 maps = maps,
                 result = response
            };
        }

        public async Task<FlexibleScheduleImportValidateResponse> ValidateFlexibleAsync(
        List<FlexibleScheduleImportDto> schedules,
        int campusId,
        DateTimeOffset startTime,
        DateTimeOffset endTime,
        Guid? importBatchId,
        CancellationToken ct,
        bool isAllowCreateImportData = false)
        {

            // 1. Init
            var response = InitFlexibleResponse(schedules);

            // 2. Prefetch
            var maps = await BuildFlexibleMapsAsync(schedules, campusId, startTime, endTime, importBatchId, ct);

            // 3. Row validation
            ValidateFlexibleRows(schedules, maps, startTime, endTime, response);

            // 4. Duplicate
            CheckFlexibleDuplicate(schedules, maps, response);

            // 5. Build conflict data
            var conflictData = BuildFlexibleConflictData(schedules, importBatchId, maps);

            // 6. Lecturer conflict
            CheckFlexibleLecturerConflict(conflictData, response);

            // 7. Room conflict
            CheckFlexibleRoomConflict(conflictData, maps, response);
            if (isAllowCreateImportData && response.CanCommit)
            {
                // 8. Convert to Schedule 
                ConvertToFlexibleScheduleEntites(maps, response);
            }
            return new FlexibleScheduleImportValidateResponse
            {
                maps = maps,
                result = response
            };
        }

        private ImportValidationResult<ScheduleImportDto, Schedule> InitResponse(List<ScheduleImportDto> schedules)
        {
            var response = new ImportValidationResult<ScheduleImportDto, Schedule>();

            for (int i = 0; i < schedules.Count; i++)
            {
                schedules[i].Index = i + 1;
                schedules[i].IsValid = true;
                schedules[i].IsUpdated = false;

                response.Rows.Add(new RowResult<ScheduleImportDto, Schedule>
                {
                    RowNumber = i,
                    Data = schedules[i],
                    Errors = new List<RowError>()
                });
            }

            return response;
        }

        private ImportValidationResult<FlexibleScheduleImportDto, Schedule> InitFlexibleResponse(List<FlexibleScheduleImportDto> schedules)
        {
            var response = new ImportValidationResult<FlexibleScheduleImportDto, Schedule>();

            for (int i = 0; i < schedules.Count; i++)
            {
                schedules[i].Index = i + 1;
                schedules[i].IsValid = true;
                schedules[i].IsUpdated = false;

                response.Rows.Add(new RowResult<FlexibleScheduleImportDto, Schedule>
                {
                    RowNumber = i,
                    Data = schedules[i],
                    Errors = new List<RowError>()
                });
            }

            return response;
        }


        private void ConvertToScheduleEntites(ImportMaps maps, ImportValidationResult<ScheduleImportDto, Schedule> response)
        {
            foreach (var row in response.Rows)
            {
                if (!row.Data.IsUpdated)
                {
                    var frame = maps.SlotTypeMap[row.Data.SlotTypeCode].SlotFrames.First(f => f.OrderIndex == row.Data.SlotOrder);
                    var startTime = row.Data.Date.ToUtcDateTimeOffset(frame.StartTimeSlot);
                    var endTime = row.Data.Date.ToUtcDateTimeOffset(frame.EndTimeSlot);
                    var newScheduleEntity = new Schedule
                    {
                        GroupId = maps.GroupMap[row.Data.GroupName].Id,
                        LabRoomId = maps.RoomMap[row.Data.RoomNo.Trim().TrimEnd('.')].Id,
                        LecturerId = maps.LecturerMap[row.Data.Lecturer].Id,
                        SlotTypeId = maps.SlotTypeMap[row.Data.SlotTypeCode].Id,
                        SubjectCode = row.Data.SubjectCode,
                        ImportHash = GenerateHash(row.Data),
                        StartTime = startTime,
                        EndTime = endTime,
                    };
                    row.ConvertedEntity = newScheduleEntity;
                }
                else
                {
                    var updatedScheduleEntity = maps.ExistingSchedules.First(s =>
                        s.ImportHash == GenerateHash(row.Data));
                    row.ConvertedEntity = new Schedule
                    {
                        Id = updatedScheduleEntity.Id,
                        GroupId = updatedScheduleEntity.GroupId,
                        LabRoomId = updatedScheduleEntity.LabRoomId,
                        LecturerId = updatedScheduleEntity.LecturerId,
                        SlotTypeId = updatedScheduleEntity.SlotTypeId,
                        SubjectCode = updatedScheduleEntity.SubjectCode,
                        ScheduleStatus = updatedScheduleEntity.ScheduleStatus,
                        ScheduleType = updatedScheduleEntity.ScheduleType,
                        ImportHash = updatedScheduleEntity.ImportHash,
                        StartTime = updatedScheduleEntity.StartTime,
                        EndTime = updatedScheduleEntity.EndTime,
                        CalendarEventId = updatedScheduleEntity.CalendarEventId,
                        StudentCount = updatedScheduleEntity.StudentCount,
                        CreatedAt = updatedScheduleEntity.CreatedAt,
                        CreatedBy = updatedScheduleEntity.CreatedBy,
                        IsActive = updatedScheduleEntity.IsActive,
                        IsDeleted = updatedScheduleEntity.IsDeleted,
                    };
                }
            }
        }

        private void ConvertToFlexibleScheduleEntites(ImportMaps maps, ImportValidationResult<FlexibleScheduleImportDto, Schedule> response)
        {
            foreach (var row in response.Rows)
            {
                if (!row.Data.IsUpdated)
                {
                    var startTime = row.Data.Date.ToUtcDateTimeOffset(row.Data.StartTime.StringToVietnamTimeOnly());
                    var endTime = row.Data.Date.ToUtcDateTimeOffset(row.Data.EndTime.StringToVietnamTimeOnly());
                    var newScheduleEntity = new Schedule
                    {
                        GroupId = maps.GroupMap[row.Data.GroupName].Id,
                        LabRoomId = maps.RoomMap[row.Data.RoomNo.Trim().TrimEnd('.')].Id,
                        LecturerId = maps.LecturerMap[row.Data.Lecturer].Id,
                        SubjectCode = row.Data.SubjectCode,
                        ImportHash = GenerateFlexibleHash(row.Data),
                        StartTime = startTime,
                        EndTime = endTime,
                    };
                    row.ConvertedEntity = newScheduleEntity;
                }
                else
                {
                    var updatedScheduleEntity = maps.ExistingSchedules.First(s =>
                        s.ImportHash == GenerateFlexibleHash(row.Data));
                    row.ConvertedEntity = new Schedule
                    {
                        Id = updatedScheduleEntity.Id,
                        GroupId = updatedScheduleEntity.GroupId,
                        LabRoomId = updatedScheduleEntity.LabRoomId,
                        LecturerId = updatedScheduleEntity.LecturerId,
                        SlotTypeId = updatedScheduleEntity.SlotTypeId,
                        SubjectCode = updatedScheduleEntity.SubjectCode,
                        ScheduleStatus = updatedScheduleEntity.ScheduleStatus,
                        ScheduleType = updatedScheduleEntity.ScheduleType,
                        ImportHash = updatedScheduleEntity.ImportHash,
                        StartTime = updatedScheduleEntity.StartTime,
                        EndTime = updatedScheduleEntity.EndTime,
                        CalendarEventId = updatedScheduleEntity.CalendarEventId,
                        StudentCount = updatedScheduleEntity.StudentCount,
                        CreatedAt = updatedScheduleEntity.CreatedAt,
                        CreatedBy = updatedScheduleEntity.CreatedBy,
                        IsActive = updatedScheduleEntity.IsActive,
                        IsDeleted = updatedScheduleEntity.IsDeleted,
                    };
                }
            }
        }

        private async Task<ImportMaps> BuildMapsAsync(
            List<ScheduleImportDto> schedules,
            int campusId,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            Guid? importBatchId,
            CancellationToken ct)
        {

            foreach (var schedule in schedules)
            {
                schedule.Lecturer = FormatHelper.Normalize(schedule.Lecturer);
                schedule.RoomNo = FormatHelper.Normalize(schedule.RoomNo);
                schedule.GroupName = FormatHelper.Normalize(schedule.GroupName);
                schedule.SubjectCode = FormatHelper.Normalize(schedule.SubjectCode);
                schedule.SlotTypeCode = FormatHelper.Normalize(schedule.SlotTypeCode);
            }
            var slotTypeCodes = schedules.Select(s => s.SlotTypeCode).Distinct().ToList();
            var roomCodes = schedules.Select(s => s.RoomNo).Distinct().ToList();
            var lecturerCodes = schedules.Select(s => s.Lecturer).Distinct().ToList();
            var groupNames = schedules.Select(s => s.GroupName).Distinct().ToList();

            var slotTypeMap = await _unitOfWork.Repository<SlotType>().Entities
                .Include(st => st.SlotFrames)
                .Where(st => st.CampusId == campusId && slotTypeCodes.Contains(st.Code))
                .ToDictionaryAsync(st => st.Code, st => st, ct);

            var roomMap = await _unitOfWork.Repository<LabRoom>().Entities
                .Include(r => r.RoomPolicies)
                .Where(r => r.Building.CampusId == campusId && roomCodes.Contains(r.RoomNo))
                .ToDictionaryAsync(r => r.RoomNo, r => r, ct);

            var lecturerMap = await _unitOfWork.Repository<User>().Entities
                .Where(u => u.CampusId == campusId && lecturerCodes.Contains(u.UserCode)) 
                .ToDictionaryAsync(u => u.UserCode, u => u, ct);

            var groupMap = await _unitOfWork.Repository<Group>().Entities
                .Where(g => groupNames.Contains(g.GroupName))
                .ToDictionaryAsync(g => g.GroupName, g => g, ct);

            var existingSchedules = await _unitOfWork.Repository<Schedule>().Entities
                .Include(s => s.LabRoom)
                .Include(s => s.User)
                .Where(s => s.LabRoom.Building.CampusId == campusId && startTime < s.EndTime && s.StartTime < endTime)
                .ToListAsync(ct);

            var subjectCodeHashes = await _unitOfWork.Repository<Subject>().Entities
                .Select(s => s.SubjectCode)
                .ToHashSetAsync();
            
            ImportBatch? importBatch = null;
            if (importBatchId != null)
            {
                importBatch = await _unitOfWork.Repository<ImportBatch>().Entities
                    .FirstOrDefaultAsync(ib => ib.Id == importBatchId, ct);
            }


            return new ImportMaps
            {
                SlotTypeMap = slotTypeMap,
                RoomMap = roomMap,
                LecturerMap = lecturerMap,
                GroupMap = groupMap,
                ExistingSchedules = existingSchedules,
                SubjectCodeHashes = subjectCodeHashes,
                ImportBatch = importBatch
            };
        }

        private async Task<ImportMaps> BuildFlexibleMapsAsync(
            List<FlexibleScheduleImportDto> schedules,
            int campusId,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            Guid? importBatchId,
            CancellationToken ct)
        {
            foreach(var schedule in schedules)
            {
                schedule.Lecturer = FormatHelper.Normalize(schedule.Lecturer);
                schedule.RoomNo = FormatHelper.Normalize(schedule.RoomNo);
                schedule.GroupName = FormatHelper.Normalize(schedule.GroupName);
                schedule.SubjectCode = FormatHelper.Normalize(schedule.SubjectCode);
            }
            var roomCodes = schedules.Select(s => s.RoomNo).Distinct().ToList();
            var lecturerCodes = schedules.Select(s => s.Lecturer).Distinct().ToList();
            var groupNames = schedules.Select(s => s.GroupName).Distinct().ToList();

            var roomMap = await _unitOfWork.Repository<LabRoom>().Entities
                .Include(r => r.RoomPolicies)
                .Where(r => r.Building.CampusId == campusId && roomCodes.Contains(r.RoomNo))
                .ToDictionaryAsync(r => r.RoomNo, r => r, ct);

            var lecturerMap = await _unitOfWork.Repository<User>().Entities
                .Where(u => u.CampusId == campusId && lecturerCodes.Contains(u.UserCode))
                .ToDictionaryAsync(u => u.UserCode, u => u, ct);

            var groupMap = await _unitOfWork.Repository<Group>().Entities
                .Where(g => groupNames.Contains(g.GroupName))
                .ToDictionaryAsync(g => g.GroupName, g => g, ct);

            var existingSchedules = await _unitOfWork.Repository<Schedule>().Entities
                .Include(s => s.LabRoom)
                .Include(s => s.User)
                .Where(s => s.LabRoom.Building.CampusId == campusId && startTime < s.EndTime && s.StartTime < endTime)
                .ToListAsync(ct);
            var subjectCodeHashes = await _unitOfWork.Repository<Subject>().Entities
                .Select(s => s.SubjectCode)
                .ToHashSetAsync();

            ImportBatch? importBatch = null;
            if (importBatchId != null)
            {
                importBatch = await _unitOfWork.Repository<ImportBatch>().Entities
                    .FirstOrDefaultAsync(ib => ib.Id == importBatchId, ct);
            }
            return new ImportMaps
            {
                RoomMap = roomMap,
                LecturerMap = lecturerMap,
                GroupMap = groupMap,
                ExistingSchedules = existingSchedules,
                SubjectCodeHashes = subjectCodeHashes,
                ImportBatch = importBatch
            };
        }
        private void ValidateRows(
            List<ScheduleImportDto> schedules,
            ImportMaps maps,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            ImportValidationResult<ScheduleImportDto, Schedule> response)
        {
            foreach (var item in schedules)
            {
                if (item.IsUpdated) continue;

                var errors = CheckSingleRow(item, maps,startTime, endTime);

                response.Rows[item.Index - 1].Errors.AddRange(errors);
            }
        }
        private void ValidateFlexibleRows(
            List<FlexibleScheduleImportDto> schedules,
            ImportMaps maps,
            DateTimeOffset startTime,
            DateTimeOffset endTime,
            ImportValidationResult<FlexibleScheduleImportDto, Schedule> response)
        {
            foreach (var item in schedules)
            {
                if (item.IsUpdated) continue;

                var errors = CheckFlexibleSingleRow(item, maps, startTime, endTime);

                response.Rows[item.Index - 1].Errors.AddRange(errors);
            }
        }
        private List<RowError> CheckSingleRow(
            ScheduleImportDto item,
            ImportMaps maps,
            DateTimeOffset startTime,
            DateTimeOffset endTime)
        {
            var errors = new List<RowError>();
            var normalizedRoomCode = item.RoomNo.Trim();

            // --- A. Check SlotTypeCode & SlotOrder ---

            if (!maps.SlotTypeMap.TryGetValue(item.SlotTypeCode, out var slotType))
            {
                errors.Add(new RowError
                {
                    FieldName = "SlotTypeCode",
                    Message = "Mã slot không tồn tại",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }
            else
            {
                var targetFrame = slotType.SlotFrames.FirstOrDefault(f => f.OrderIndex == item.SlotOrder);
                if (targetFrame == null)
                {
                    errors.Add(new RowError
                    {
                        FieldName = "SlotOrder",
                        Message = "Slot Order không tồn tại",
                        Severity = ErrorSeverity.Error
                    });
                    item.IsValid = false;
                }
            }

            // --- B. Check LabRoom ---
            if (!maps.RoomMap.TryGetValue(normalizedRoomCode, out var room))
            {
                errors.Add(new RowError
                {
                    FieldName = "RoomNo",
                    Message = "Phòng không tồn tại trên hệ thống",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }

            // --- C. Check Lecturer ---
            if (!maps.LecturerMap.ContainsKey(item.Lecturer))
            {
                errors.Add(new RowError
                {
                    FieldName = "Lecturer",
                    Message = "Giảng viên không tồn tại trên hệ thống",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }

            // --- D. Check GroupName ---
            if (!maps.GroupMap.ContainsKey(item.GroupName))
            {
                errors.Add(new RowError
                {
                    FieldName = "GroupName",
                    Message = "Tên nhóm không tồn tại trên hệ thống",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }

            // --- E. Check DateTime Format ---
            if (!DateTime.TryParseExact(item.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var sessionDate))
            {
                errors.Add(new RowError
                {
                    FieldName = "Date",
                    Message = "Ngày học không hợp lệ",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }
            else if(sessionDate < startTime || sessionDate > endTime)
            {
                errors.Add(new RowError
                {
                    FieldName = "Date",
                    Message = "Ngày học không nằm trong kì yêu cầu",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }

            // --- F. Check SubjectCode ---
            if (!maps.SubjectCodeHashes.Contains(item.SubjectCode))
            {
                errors.Add(new RowError
                {
                    FieldName = "SubjectCode",
                    Message = "Mã môn học không tồn tại trên hệ thống",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }

            return errors;
        }

        private List<RowError> CheckFlexibleSingleRow(
            FlexibleScheduleImportDto item,
            ImportMaps maps,
            DateTimeOffset startTime,
            DateTimeOffset endTime)
        {
            var errors = new List<RowError>();
            var normalizedRoomCode = item.RoomNo.Trim();

            // --- A. Check SlotTypeCode & SlotOrder ---
            var isValidStartTime = TimeOnly.TryParseExact(item.StartTime.Trim(), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly startTimeOnly);
            if (!isValidStartTime)
            {
                errors.Add(new RowError
                {
                    FieldName = "StartTime",
                    Message = "Thời gian bắt đầu không hợp lệ",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }

            var isValidEndTime = TimeOnly.TryParseExact(item.EndTime.Trim(), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out TimeOnly endTimeOnly);
            if (!isValidEndTime)
            {
                errors.Add(new RowError
                {
                    FieldName = "EndTime",
                    Message = "Thời gian kết thúc không hợp lệ",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }
            if(isValidStartTime && isValidEndTime && endTimeOnly <= startTimeOnly)
            {
                errors.Add(new RowError
                {
                    FieldName = "EndTime",
                    Message = "Thời gian kết thúc phải sau thời gian bắt đầu",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }

            // --- B. Check LabRoom ---
            if (!maps.RoomMap.TryGetValue(normalizedRoomCode, out var room))
            {
                errors.Add(new RowError
                {
                    FieldName = "RoomNo",
                    Message = "Phòng không tồn tại trên hệ thống",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }

            // --- C. Check Lecturer ---
            if (!maps.LecturerMap.ContainsKey(item.Lecturer))
            {
                errors.Add(new RowError
                {
                    FieldName = "Lecturer",
                    Message = "Giảng viên không tồn tại trên hệ thống",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }

            // --- D. Check GroupName ---
            if (!maps.GroupMap.ContainsKey(item.GroupName))
            {
                errors.Add(new RowError
                {
                    FieldName = "GroupName",
                    Message = "Tên nhóm không tồn tại trên hệ thống",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }

            // --- E. Check DateTime Format ---
            if (!DateTime.TryParseExact(item.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var sessionDate))
            {
                errors.Add(new RowError
                {
                    FieldName = "Date",
                    Message = "Ngày học không hợp lệ",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }
            else if (sessionDate < startTime || sessionDate > endTime)
            {
                errors.Add(new RowError
                {
                    FieldName = "Date",
                    Message = "Ngày học không nằm trong kì yêu cầu",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }
            // --- F. Check SubjectCode ---
            if (!maps.SubjectCodeHashes.Contains(item.SubjectCode))
            {
                errors.Add(new RowError
                {
                    FieldName = "SubjectCode",
                    Message = "Mã môn học không tồn tại trên hệ thống",
                    Severity = ErrorSeverity.Error
                });
                item.IsValid = false;
            }

            return errors;
        }

        private void CheckDuplicate(
            List<ScheduleImportDto> schedules,
            ImportMaps maps,
            ImportValidationResult<ScheduleImportDto, Schedule> response)
        {
            var existingScheduleDictionary = maps.ExistingSchedules
                .Where(s => s.ImportHash != null) // Chỉ xét các lịch đã được import trước đó
                .ToDictionary(s => s.ImportHash!, s => s);

            // Dùng Dictionary: Key là Hash, Value là Index (số dòng) của lần đầu xuất hiện
            var processedInFileTracker = new Dictionary<string, int>();

            foreach (var item in schedules)
            {
                if (!item.IsValid) continue;
                var hash = GenerateHash(item);

                // --- Kiểm tra trùng lặp NỘI BỘ trong file ---
                if (processedInFileTracker.TryGetValue(hash, out int originalLineIndex))
                {
                    item.IsValid = false;
                    response.Rows[item.Index - 1].Errors.Add(new RowError
                    {
                        FieldName = "RoomNo",
                        // Thông báo cụ thể dòng bị trùng
                        Message = $"Dòng này bị trùng lặp với dòng thứ {originalLineIndex} trong file Excel.",
                        Severity = ErrorSeverity.Error
                    });
                    continue;
                }

                // --- Kiểm tra trùng lặp với Database ---
                if (existingScheduleDictionary.TryGetValue(hash, out var existingSchedule))
                {
                    if(maps.ImportBatch == null)
                    {
                        // Nếu importBatchId là null, tức là không cho phép cập nhật, thì đánh lỗi
                        item.IsValid = false;
                        response.Rows[item.Index - 1].Errors.Add(new RowError
                        {
                            FieldName = "RoomNo",
                            Message = "Lịch đã tồn tại trên hệ thống.",
                            Severity = ErrorSeverity.Error
                        });
                        continue;
                    }
                    else if(existingSchedule.ImportBatchId != maps.ImportBatch.Id) 
                    {
                         item.IsValid = false;
                         response.Rows[item.Index - 1].Errors.Add(new RowError
                         {
                             FieldName = "RoomNo",
                             Message = $"Lịch đã tồn tại trên batch {maps.ImportBatch.Name}",
                             Severity = ErrorSeverity.Error
                         });
                         continue;

                    }
                    item.IsUpdated = true;
                    response.Rows[item.Index - 1].Errors.Add(new RowError
                    {
                        FieldName = "RoomNo",
                        Message = "Lịch đã tồn tại trên hệ thống (Sẽ được cập nhật)",
                        Severity = ErrorSeverity.Warning
                    });
                }

                // Lưu lại Hash và Index của dòng này để các dòng sau đối chiếu
                processedInFileTracker.Add(hash, item.Index);
            }
        }

        private void CheckFlexibleDuplicate(
    List<FlexibleScheduleImportDto> schedules,
    ImportMaps maps,
    ImportValidationResult<FlexibleScheduleImportDto, Schedule> response)
        {
            var existingScheduleDictionary = maps.ExistingSchedules
    .Where(s => s.ImportHash != null) // Chỉ xét các lịch đã được import trước đó
    .ToDictionary(s => s.ImportHash!, s => s);

            // Lưu vết: <Hash, Số dòng trong Excel>
            var processedInFileTracker = new Dictionary<string, int>();

            foreach (var item in schedules)
            {
                if (!item.IsValid) continue;
                var hash = GenerateFlexibleHash(item);

                if (processedInFileTracker.TryGetValue(hash, out int originalLineIndex))
                {
                    item.IsValid = false;
                    response.Rows[item.Index - 1].Errors.Add(new RowError
                    {
                        FieldName = "RoomNo",
                        Message = $"Dòng này có dữ liệu giống hệt dòng thứ {originalLineIndex} phía trên.",
                        Severity = ErrorSeverity.Error
                    });
                    continue;
                }

                if (existingScheduleDictionary.TryGetValue(hash, out var existingSchedule))
                {
                    if (maps.ImportBatch == null)
                    {
                        // Nếu importBatchId là null, tức là không cho phép cập nhật, thì đánh lỗi
                        item.IsValid = false;
                        response.Rows[item.Index - 1].Errors.Add(new RowError
                        {
                            FieldName = "RoomNo",
                            Message = "Lịch đã tồn tại trên hệ thống.",
                            Severity = ErrorSeverity.Error
                        });
                        continue;
                    }
                    else if (existingSchedule.ImportBatchId != maps.ImportBatch.Id)
                    {
                        item.IsValid = false;
                        response.Rows[item.Index - 1].Errors.Add(new RowError
                        {
                            FieldName = "RoomNo",
                            Message = $"Lịch đã tồn tại trên batch {maps.ImportBatch.Name}",
                            Severity = ErrorSeverity.Error
                        });
                        continue;

                    }
                    item.IsUpdated = true;
                    response.Rows[item.Index - 1].Errors.Add(new RowError
                    {
                        FieldName = "RoomNo",
                        Message = "Lịch đã tồn tại trên hệ thống (Sẽ được cập nhật)",
                        Severity = ErrorSeverity.Warning
                    });
                }

                processedInFileTracker.Add(hash, item.Index);
            }
        }

        private List<ConflictScheduleDto> BuildConflictData(
            List<ScheduleImportDto> schedules,
            Guid? importBatchId,
            ImportMaps maps)
        {
            var db = maps.ExistingSchedules
                .Where(s => importBatchId == null || s.ImportBatchId != importBatchId)
                .Select(s =>
            {
                return new ConflictScheduleDto
                {
                    Lecturer = s.User.UserCode,
                    RoomNo = s.LabRoom.RoomNo,
                    Date = s.StartTime.UtcToVietnamDateOnly(),
                    Start = s.StartTime.ToVietnamTimeOnly(),
                    End = s.EndTime.ToVietnamTimeOnly(),
                    Source = "DB",
                    RefId = s.Id,
                    ImportBatchId = s.ImportBatchId,
                    Index = -1
                };
            });

            var import = schedules.Where(s => s.IsValid && !s.IsUpdated).Select(s =>
            {
                var frame = maps.SlotTypeMap[s.SlotTypeCode].SlotFrames.First(f => f.OrderIndex == s.SlotOrder);
                return new ConflictScheduleDto
                {
                    Lecturer = s.Lecturer,
                    RoomNo = s.RoomNo,
                    Date = s.Date.StringToVietnamDateOnly(),
                    Start = frame.StartTimeSlot,
                    End = frame.EndTimeSlot,
                    Source = "Import",
                    RefId = -1,
                    Index = s.Index
                };
            }).ToList();

            return db.Concat(import).ToList();
        }

        private List<ConflictScheduleDto> BuildFlexibleConflictData(
            List<FlexibleScheduleImportDto> schedules,
            Guid? importBatchId,
            ImportMaps maps)
        {
            var db = maps.ExistingSchedules
                .Where(s => importBatchId == null || s.ImportBatchId != importBatchId)
                .Select(s =>
            {
                return new ConflictScheduleDto
                {
                    Lecturer = s.User.UserCode,
                    RoomNo = s.LabRoom.RoomNo,
                    Date = s.StartTime.UtcToVietnamDateOnly(),
                    Start = s.StartTime.ToVietnamTimeOnly(),
                    End = s.EndTime.ToVietnamTimeOnly(),
                    Source = "DB",
                    RefId = s.Id,
                    Index = -1
                };
            });

            var import = schedules.Where(s => s.IsValid && !s.IsUpdated).Select(s =>
            {
                return new ConflictScheduleDto
                {
                    Lecturer = s.Lecturer,
                    RoomNo = s.RoomNo,
                    Date = s.Date.StringToVietnamDateOnly(),
                    Start = s.StartTime.StringToVietnamTimeOnly(),
                    End = s.EndTime.StringToVietnamTimeOnly(),
                    Source = "Import",
                    RefId = -1,
                    Index = s.Index
                };
            }).ToList();

            return db.Concat(import).ToList();
        }

        public void CheckLecturerConflict(List<ConflictScheduleDto> conflictData, ImportValidationResult<ScheduleImportDto, Schedule> response)
        {
            var lecturerGroups = conflictData
                .GroupBy(s => new
                {
                     s.Lecturer,
                     s.Date
                });

            foreach (var group in lecturerGroups)
            {
                var dateConflictSchedules = group.OrderBy(g => g.Start).ToList();

                for (int i = 0; i < dateConflictSchedules.Count - 1; i++)
                {
                    var current = dateConflictSchedules[i];
                    var next = dateConflictSchedules[i + 1];
                    if (current.End > next.Start)
                    {
                        if (current.Source == "Import" && next.Source == "Import")
                        {
                            response.Rows[current.Index - 1].Errors.Add(new RowError
                            {
                                FieldName = "Lecturer",
                                Message = $"Xung đột với lịch dạy với record {next.Index}",
                                Severity = ErrorSeverity.Error,
                            });
                            response.Rows[next.Index - 1].Errors.Add(new RowError
                            {
                                FieldName = "Lecturer",
                                Message = $"Xung đột với lịch dạy với record {current.Index}",
                                Severity = ErrorSeverity.Error,
                            });
                        }
                        else if (current.Source == "Import" && next.Source == "DB")
                        {
                            response.Rows[current.Index - 1].Errors.Add(new RowError
                            {
                                FieldName = "Lecturer",
                                Message = $"Xung đột với lịch dạy với ScheduleId {next.RefId} thời gian {next.Date} {next.Start}-{next.End}",
                                Severity = ErrorSeverity.Error,
                            });
                        }

                        else if (current.Source == "DB" && next.Source == "Import")
                        {
                            response.Rows[next.Index - 1].Errors.Add(new RowError
                            {
                                FieldName = "Lecturer",
                                Message = $"Xung đột với lịch dạy với ScheduleId {current.RefId} thời gian {current.Date} {current.Start}-{current.End}",
                                Severity = ErrorSeverity.Error,
                            });
                        }

                    }
                }
            }
        }

        public void CheckRoomConflict(List<ConflictScheduleDto> conflictData, ImportMaps maps, ImportValidationResult<ScheduleImportDto, Schedule> response)
        {
            var slotGroups = conflictData
                .GroupBy(s => new
                {
                    s.RoomNo,
                    s.Date
                });
            foreach (var group in slotGroups)
            {
                if (!maps.RoomMap.TryGetValue(group.Key.RoomNo, out var roomTarget))
                    continue;
                var maxConcurrent = roomTarget.RoomPolicies.FirstOrDefault(rp => rp.PolicyKey == PolicyType.MaxConcurrentBookings)?.PolicyValue ?? "1";
                var maxConcurrentInt = int.TryParse(maxConcurrent, out var result) ? result : 1;

                var timePoints = new List<(ConflictScheduleDto conflictScheduleDto, TimeOnly time, int Change)>();
                foreach (var schedule in group)
                {
                    timePoints.Add((schedule, schedule.Start, +1));
                    timePoints.Add((schedule, schedule.End, -1));
                }
                timePoints = timePoints.OrderBy(tp => tp.time).ToList();
                var conflictScheduleTracker = new List<ConflictScheduleDto>();
                int concurrentCount = 0;
                ConflictScheduleDto latestImportSchedule = null;
                foreach (var tp in timePoints)
                {
                    concurrentCount += tp.Change;
                    if (tp.Change > 0)
                    {
                        // Starting a schedule
                        if (tp.conflictScheduleDto.Source == "Import")
                        {
                            latestImportSchedule = tp.conflictScheduleDto;
                        }

                        conflictScheduleTracker.Add(tp.conflictScheduleDto);
                        if (concurrentCount > maxConcurrentInt && latestImportSchedule != null)
                        {
                            var conflictContextList = conflictScheduleTracker
                                .Where(cs => cs.Index != latestImportSchedule.Index)
                                .Select(cs => $"{(cs.Source == "DB" ? "ScheduleId: " + cs.RefId.ToString() : "Row: " + cs.Index.ToString())}, Time: {cs.Date} {cs.Start}-{cs.End}").ToList();
                            var conflictContext = $"Vượt số lượng schedule với {string.Join("; ", conflictContextList)}";
                            response.Rows[latestImportSchedule.Index - 1].Errors.Add(new RowError
                            {
                                FieldName = "RoomNo",
                                Message = conflictContext,
                                Severity = ErrorSeverity.Error,
                            });

                        }

                    }
                    else
                    {
                        conflictScheduleTracker.Remove(tp.conflictScheduleDto);
                    }
                }
            }
        }

        public void CheckFlexibleLecturerConflict(List<ConflictScheduleDto> conflictData, ImportValidationResult<FlexibleScheduleImportDto, Schedule> response)
        {
            var lecturerGroups = conflictData
                .GroupBy(s => new
                {
                    s.Lecturer,
                    s.Date
                });

            foreach (var group in lecturerGroups)
            {
                var dateConflictSchedules = group.OrderBy(g => g.Start).ToList();

                for (int i = 0; i < dateConflictSchedules.Count - 1; i++)
                {
                    var current = dateConflictSchedules[i];
                    var next = dateConflictSchedules[i + 1];
                    if (current.End > next.Start)
                    {
                        if (current.Source == "Import" && next.Source == "Import")
                        {
                            response.Rows[current.Index - 1].Errors.Add(new RowError
                            {
                                FieldName = "Lecturer",
                                Message = $"Xung đột với lịch dạy với record {next.Index}",
                                Severity = ErrorSeverity.Error,
                            });
                            response.Rows[next.Index - 1].Errors.Add(new RowError
                            {
                                FieldName = "Lecturer",
                                Message = $"Xung đột với lịch dạy với record {current.Index}",
                                Severity = ErrorSeverity.Error,
                            });
                        }
                        else if (current.Source == "Import" && next.Source == "DB")
                        {
                            response.Rows[current.Index - 1].Errors.Add(new RowError
                            {
                                FieldName = "Lecturer",
                                Message = $"Xung đột với lịch dạy với ScheduleId {next.RefId} thời gian {next.Date} {next.Start}-{next.End}",
                                Severity = ErrorSeverity.Error,
                            });
                        }

                        else if (current.Source == "DB" && next.Source == "Import")
                        {
                            response.Rows[next.Index - 1].Errors.Add(new RowError
                            {
                                FieldName = "Lecturer",
                                Message = $"Xung đột với lịch dạy với ScheduleId {current.RefId} thời gian {current.Date} {current.Start}-{current.End}",
                                Severity = ErrorSeverity.Error,
                            });
                        }

                    }
                }
            }
        }

        public void CheckFlexibleRoomConflict(List<ConflictScheduleDto> conflictData, ImportMaps maps, ImportValidationResult<FlexibleScheduleImportDto, Schedule> response)
        {
            var slotGroups = conflictData
                .GroupBy(s => new
                {
                    s.RoomNo,
                    s.Date
                });
            foreach (var group in slotGroups)
            {
                if (!maps.RoomMap.TryGetValue(group.Key.RoomNo, out var roomTarget))
                    continue;
                var maxConcurrent = roomTarget.RoomPolicies.FirstOrDefault(rp => rp.PolicyKey == PolicyType.MaxConcurrentBookings)?.PolicyValue ?? "1";
                var maxConcurrentInt = int.TryParse(maxConcurrent, out var result) ? result : 1;

                var timePoints = new List<(ConflictScheduleDto conflictScheduleDto, TimeOnly time, int Change)>();
                foreach (var schedule in group)
                {
                    timePoints.Add((schedule, schedule.Start, +1));
                    timePoints.Add((schedule, schedule.End, -1));
                }
                timePoints = timePoints.OrderBy(tp => tp.time).ToList();
                var conflictScheduleTracker = new List<ConflictScheduleDto>();
                int concurrentCount = 0;
                ConflictScheduleDto latestImportSchedule = null;
                foreach (var tp in timePoints)
                {
                    concurrentCount += tp.Change;
                    if (tp.Change > 0)
                    {
                        // Starting a schedule
                        if (tp.conflictScheduleDto.Source == "Import")
                        {
                            latestImportSchedule = tp.conflictScheduleDto;
                        }

                        conflictScheduleTracker.Add(tp.conflictScheduleDto);
                        if (concurrentCount > maxConcurrentInt && latestImportSchedule != null)
                        {
                            var conflictContextList = conflictScheduleTracker
                                .Where(cs => cs.Index != latestImportSchedule.Index)
                                .Select(cs => $"{(cs.Source == "DB" ? "ScheduleId: " + cs.RefId.ToString() : "Row: " + cs.Index.ToString())}, Time: {cs.Date} {cs.Start}-{cs.End}").ToList();
                            var conflictContext = $"Vượt số lượng schedule với {string.Join("; ", conflictContextList)}";
                            response.Rows[latestImportSchedule.Index - 1].Errors.Add(new RowError
                            {
                                FieldName = "RoomNo",
                                Message = conflictContext,
                                Severity = ErrorSeverity.Error,
                            });
                        }
                    }
                    else
                    {
                        conflictScheduleTracker.Remove(tp.conflictScheduleDto);
                    }
                }
            }
        }

        public string GenerateHash(ScheduleImportDto d)
        {
            return $"{d.GroupName.Trim().ToLower()}_{d.Date.StringToVietnamDateOnly().ToString("yyyy-MM-dd")}_{d.SlotOrder}_{d.RoomNo.Trim().ToLower()}_{d.SlotTypeCode.Trim().ToLower()}_{d.Lecturer.Trim().ToLower()}";
        }
        public string GenerateFlexibleHash(FlexibleScheduleImportDto d)
        {
            return $"{d.GroupName.Trim().ToLower()}_{d.Date.StringToVietnamDateOnly().ToString("yyyy-MM-dd")}_{d.StartTime.StringToVietnamTimeOnly().ToString("HH:mm")}_{d.EndTime.StringToVietnamTimeOnly().ToString("HH:mm")}_{d.RoomNo.Trim().ToLower()}_{d.Lecturer.Trim().ToLower()}";
        }

    }
}
