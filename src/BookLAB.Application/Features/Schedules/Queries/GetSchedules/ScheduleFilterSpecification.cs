using BookLAB.Application.Common.Specifications;
using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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
                AddCriteria(s => s.StartTime < query.ToDate.Value);

            if (query.LecturerId.HasValue) { 
                AddCriteria(s => s.LecturerId == query.LecturerId.Value);
            }

            if (!string.IsNullOrEmpty(query.SubjectCode))
                AddCriteria(s => s.SubjectCode.Contains(query.SubjectCode));

            // eager loading related entities
            AddInclude(s => s.LabRoom);
            AddInclude(s => s.SlotType);

            // Sorting logic can be handled in the repository or service layer based on query.SortBy and query.IsDescending
            ApplyOrderByDescending(s => s.StartTime);
        }
    }
}
