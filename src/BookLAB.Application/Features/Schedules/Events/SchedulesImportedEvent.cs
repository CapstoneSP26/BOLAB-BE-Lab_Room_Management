using MediatR;

namespace BookLAB.Application.Features.Schedules.Events
{
    public class SchedulesImportedEvent : INotification
    {
        public List<Guid> ScheduleIds { get; }

        public SchedulesImportedEvent(List<Guid> scheduleIds)
        {
            ScheduleIds = scheduleIds;
        }
    }
}
