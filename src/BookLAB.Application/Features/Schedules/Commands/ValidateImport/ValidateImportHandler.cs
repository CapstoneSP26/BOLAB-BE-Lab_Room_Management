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
        private readonly IScheduleService _scheduleService;
        public ValidateImportHandler(IUnitOfWork unitOfWork, IScheduleService scheduleService)
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
                .Where(st => slotTypeCodes.Contains(st.Code))
                .ToDictionaryAsync(st => st.Code, st => st.SlotFrames.ToList(), cancellationToken);

            // Get Map From LabRoom
            var roomMap = await _unitOfWork.Repository<LabRoom>().Entities
                .Where(r => roomCodes.Contains(r.RoomNo))
                .ToDictionaryAsync(r => r.RoomNo, r => r, cancellationToken);

            // Get Map Lecturer 
            var lecturerMap = await _unitOfWork.Repository<User>().Entities
                .Where(u => lecturerNames.Contains(u.UserCode))
                .ToDictionaryAsync(u => u.UserCode, u => u, cancellationToken);

            // Get Map Group 
            var groupMap = await _unitOfWork.Repository<Group>().Entities
                .Where(g => groupNames.Contains(g.GroupName))
                .ToDictionaryAsync(g => g.GroupName, g => g, cancellationToken);

            // 2. VALIDATION LOOP
            foreach (var item in request.Schedules)
            {
                var rowResult = _scheduleService.CheckSingleRowAsync(item, roomMap, lecturerMap, groupMap, slotTypeMap, cancellationToken);
                response.Rows.Add(rowResult);
            }

            return response;
        }
    
    
    }
}
