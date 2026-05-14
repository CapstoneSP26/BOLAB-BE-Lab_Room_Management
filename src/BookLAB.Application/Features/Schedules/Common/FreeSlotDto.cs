using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Schedules.Common
{
    public class FreeSlotDto
    {
        public int RoomId { get; set; }
        public int? BuildingId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
    }
}
