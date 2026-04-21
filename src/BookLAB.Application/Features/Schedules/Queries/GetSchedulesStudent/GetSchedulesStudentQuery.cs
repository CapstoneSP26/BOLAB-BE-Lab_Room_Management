using BookLAB.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.GetSchedulesStudent
{
    public class GetSchedulesStudentQuery : IRequest<List<ScheduleDto2>>
    {
        public DateTimeOffset FromDate { get; set; }
        public DateTimeOffset ToDate { get; set; }
        public bool Status { get; set; }
    }
}
