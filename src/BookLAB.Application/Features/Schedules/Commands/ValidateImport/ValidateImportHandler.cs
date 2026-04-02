using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BookLAB.Application.Features.Schedules.Commands.ValidateImport
{
    public class ValidateImportHandler : IRequestHandler<ValidateImportQuery, ImportValidationResult<ScheduleImportDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleImportService _scheduleService;
        public ValidateImportHandler(IUnitOfWork unitOfWork, IScheduleImportService scheduleService)
        {
            _unitOfWork = unitOfWork;
            _scheduleService = scheduleService;
        }
        public async Task<ImportValidationResult<ScheduleImportDto>> Handle(ValidateImportQuery request, CancellationToken cancellationToken)
        {
            var response = new ImportValidationResult<ScheduleImportDto>();
            // 1. PRE-FETCH DATA (Optimize performance, avoid N+1 Query)
            var slotTypeCodes = request.Schedules.Select(s => s.SlotTypeCode).Distinct().ToList();
            var roomCodes = request.Schedules.Select(s => s.RoomNo.Trim().TrimEnd('.')).Distinct().ToList();
            var lecturerNames = request.Schedules.Select(s => s.Lecturer).Distinct().ToList();
            var groupNames = request.Schedules.Select(s => s.GroupName).Distinct().ToList();

            // Get Map SlotType along with SlotFrame
            var slotTypeMap = await _unitOfWork.Repository<SlotType>().Entities
                .Include(st => st.SlotFrames)
                .Where(st => st.CampusId == request.CampusId && slotTypeCodes.Contains(st.Name))
                .ToDictionaryAsync(st => st.Code, st => st.SlotFrames.ToList(), cancellationToken);

            // Get Map From LabRoom
            var roomMap = await _unitOfWork.Repository<LabRoom>().Entities
                .Include(r => r.RoomPolicies)
                .Where(r => r.Building.CampusId == request.CampusId && roomCodes.Contains(r.RoomNo))
                .ToDictionaryAsync(r => r.RoomNo, r => r, cancellationToken);

            // Get Map Lecturer 
            var lecturerMap = await _unitOfWork.Repository<User>().Entities
                .Where(u => u.CampusId == request.CampusId && lecturerNames.Contains(u.UserCode))
                .ToDictionaryAsync(u => u.UserCode, u => u, cancellationToken);

            // Get Map Group 
            var groupMap = await _unitOfWork.Repository<Group>().Entities
                .Where(g => groupNames.Contains(g.GroupName))
                .ToDictionaryAsync(g => g.GroupName, g => g, cancellationToken);

            // Get All Existing Hashes to Check Duplicates
            var existingSchesule = await _unitOfWork.Repository<Schedule>().Entities
                .Where(s => s.LabRoom.Building.CampusId == request.CampusId)
                .ToListAsync(cancellationToken);

            var existingHashes = existingSchesule.Select(s => s.ImportHash).ToList();


            // 2. VALIDATION LOOP
            foreach (var item in request.Schedules)
            {
                var rowResult = _scheduleService.CheckSingleRowAsync(request.Schedules, item, roomMap, lecturerMap, groupMap, slotTypeMap, existingHashes, cancellationToken);
                response.Rows.Add(rowResult);
            }

            // 3. CHECK LECTURER IMPORT SCHEDULE CONFLICT
            var lecturerGroups = request.Schedules
                .GroupBy(s => new
                {
                     s.Lecturer,
                     Date = DateOnly.Parse(s.Date)
                });

            foreach (var group in lecturerGroups)
            {
                var schedules = group.ToList();

                // Convert sang time range
                var timeRanges = schedules.Select(s =>
                {
                    if (!slotTypeMap.TryGetValue(s.SlotTypeCode, out var frames)) return null;
                    var frame = frames.First(f => f.OrderIndex == s.SlotOrder);
                    if (frame == null) return null;

                    return new
                    {
                        Schedule = s,
                        Start = frame.StartTimeSlot,
                        End = frame.EndTimeSlot
                    };
                }).Where(x => x != null)
                  .OrderBy(x => x.Start).ToList();

                for(int i = 0; i < timeRanges.Count - 1; i++)
                {
                    var current = timeRanges[i];
                    var next = timeRanges[i + 1];
                    if(current.End > next.Start)
                    {
                        // Conflict
                        response.Rows[current.Schedule.Index - 1].Errors.Add(new RowError
                        {
                            FieldName = "Lecturer",
                            Message = $"Xung đột với lịch dạy với record {next.Schedule.Index}",
                            Severity = ErrorSeverity.Error
                        });
                        response.Rows[next.Schedule.Index - 1].Errors.Add(new RowError
                        {
                            FieldName = "Lecturer",
                            Message = $"Xung đột với lịch dạy với record {current.Schedule.Index}",
                            Severity = ErrorSeverity.Error
                        });
                    }
                }

                return response;
            }
            // 4. CHECK ROOM IMPORT SCHEDULE CONFLICT
            
        }

    }
}
