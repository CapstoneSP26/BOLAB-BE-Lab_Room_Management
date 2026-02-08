using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Domain.DTOs
{
    public class AddScheduleDTO
    {
        public Guid lecturerId { get; set; }
        public int labRoomId { get; set; }
        public string scheduleType { get; set; }
        public Guid createdBy { get; set; }
        public DateTimeOffset startTime { get; set; }
        public DateTimeOffset endTime { get; set; }
        public bool fromAdmin { get; set; }
    }
}
