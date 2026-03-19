using BookLAB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Models
{
    public class AttendanceDto
    {
        public Guid? Id { get; set; }
        public Guid ScheduleId { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset? CheckInTime { get; set; }
        public DateTimeOffset? CheckOutTime { get; set; }
        public AttendanceCheckInMethod CheckInMethod { get; set; }
        public AttendanceStatus AttendanceStatus { get; set; }
        public DateTimeOffset? CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
