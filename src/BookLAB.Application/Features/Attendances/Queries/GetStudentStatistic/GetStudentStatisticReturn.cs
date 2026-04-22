using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Attendances.Queries.GetStudentStatistic
{
    public class GetStudentStatisticReturn
    {
        public int AttendInWeek { get; set; }
        public int ClassInSemesterLeft { get; set; }
    }
}
