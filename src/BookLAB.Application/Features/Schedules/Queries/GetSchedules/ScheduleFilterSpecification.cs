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

            ApplyOrderBy(s => s.StartTime);
        }
    }
}
