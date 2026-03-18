
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Common;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Interfaces.Services
{
    public interface IScheduleService
    {
        RowResult<ScheduleImportDto> CheckSingleRowAsync(
            ScheduleImportDto item,
            Dictionary<string, LabRoom> roomMap,
            Dictionary<string, User> lecturerMap,
            Dictionary<string, Group> groupMap,
            Dictionary<string, List<SlotFrame>> slotTypeMap,
            CancellationToken ct
        );

        public Schedule ConvertToScheduleEntity(
            ScheduleImportDto item,
            Dictionary<string, LabRoom> roomMap,
            Dictionary<string, User> lecturerMap,
            Dictionary<string, Group> groupMap,
            Dictionary<string, List<SlotFrame>> slotTypeMap,
            CancellationToken ct
        );
    }
}
