using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Queries.GetAvailableSlots
{
    public class AvailableScheduleResponse
    {
        public string id { get; set; }
        public string roomId { get; set; }
        public string date { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string status { get; set; }
        public string? bookedBy { get; set; }
        public string? groupName { get; set; }
        public string slotType { get; set; }
        public int slotNumber { get; set; }
    }
}
