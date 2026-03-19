using BookLAB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Models
{
    public class ScheduleDto
    {
        public Guid LecturerId { get; set; }
        public int LabRoomId { get; set; }
        public Guid? BookingId { get; set; }
        public Guid? GroupId { get; set; }
        public int SlotTypeId { get; set; }
        public string? CalendarEventId { get; set; }
        public ScheduleType ScheduleType { get; set; }
        public ScheduleStatus ScheduleStatus { get; set; }
        public int StudentCount { get; set; }
        public string? SubjectCode { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }

        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
    }
}
