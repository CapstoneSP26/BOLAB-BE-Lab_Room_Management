using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.GetAvailableSlots
{
    public class AvailableScheduleRequest
    {
        public string roomId { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string? startTime { get; set; }
        public string? endTime { get; set; }
    }
}
