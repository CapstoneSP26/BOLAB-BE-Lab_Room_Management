
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Schedules.Commands.ImportSchedule
{
    public class ConfirmImportHandler : IRequestHandler<ConfirmImportCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleService _scheduleService;

        public ConfirmImportHandler(IUnitOfWork unitOfWork, IScheduleService scheduleService)
        {
            _unitOfWork = unitOfWork;
            _scheduleService = scheduleService;
        }

        public async Task<bool> Handle(ConfirmImportCommand request, CancellationToken cancellationToken)
        {
            // 1. PRE-FETCH DATA (Optimize performance, avoid N+1 Query)
            var slotTypeCodes = request.ValidSchedules.Select(s => s.SlotTypeCode).Distinct().ToList();
            var roomCodes = request.ValidSchedules.Select(s => s.RoomNo.Trim().TrimEnd('.')).Distinct().ToList();
            var lecturerNames = request.ValidSchedules.Select(s => s.Lecturer).Distinct().ToList();
            var groupNames = request.ValidSchedules.Select(s => s.GroupName).Distinct().ToList();

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
            var groups = await _unitOfWork.Repository<Group>().Entities
                .Where(g => groupNames.Contains(g.GroupName))
                .ToListAsync(cancellationToken);

            var ambiguousGroupNames = groups
                .GroupBy(g => g.GroupName)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var groupMap = groups
                .Where(g => !ambiguousGroupNames.Contains(g.GroupName))
                .GroupBy(g => g.GroupName)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                //  VALIDATION LOOP
                foreach (var item in request.ValidSchedules)
                {
                    if (ambiguousGroupNames.Contains(item.GroupName))
                    {
                        throw new Exception($"Tên group '{item.GroupName}' đang bị trùng trên hệ thống. Không thể import an toàn.");
                    }

                    var validation = _scheduleService.CheckSingleRowAsync(item, roomMap, lecturerMap, groupMap, slotTypeMap, cancellationToken);
                    if (validation.IsCritical)
                    {
                        throw new Exception($"Dữ liệu đã thay đổi: {string.Join(", ", validation.Messages)}");
                    }

                    var newSchedule = _scheduleService.ConvertToScheduleEntity(item, roomMap, lecturerMap, groupMap, slotTypeMap, cancellationToken);
                    await _unitOfWork.Repository<Schedule>().AddAsync(newSchedule);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();
                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }


        }
    }
}
