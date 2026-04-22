using BookLAB.Application.Common.Specifications;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Schedules.Queries.GetSchedules
{
    public class ScheduleFilterSpecification : BaseSpecification<Schedule>
    {
        public ScheduleFilterSpecification(GetSchedulesQuery query)
        {
            if(query.LabRoomId.HasValue)
                AddCriteria(s => s.LabRoomId == query.LabRoomId.Value);

            if (query.BuildingId.HasValue)
                AddCriteria(s => s.LabRoom.BuildingId == query.BuildingId.Value);

            if (query.FromDate.HasValue)
                AddCriteria(s => s.StartTime >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                AddCriteria(s => s.EndTime < query.ToDate.Value);

            if (query.LecturerId.HasValue) { 
                AddCriteria(s => s.LecturerId == query.LecturerId.Value);
            }
            if (query.Status.HasValue)
            {
                AddCriteria(s => s.ScheduleStatus == query.Status.Value);
            }
            if (!string.IsNullOrEmpty(query.SubjectCode))
                AddCriteria(s => s.SubjectCode.Contains(query.SubjectCode));

            if (query.ExcludedStatus.HasValue)
            {
                AddCriteria(s => s.ScheduleStatus != query.ExcludedStatus.Value);
            }

            AddCriteria(s => s.IsActive && !s.IsDeleted);

            ApplyOrderBy(s => s.StartTime);
        }
    }
}
