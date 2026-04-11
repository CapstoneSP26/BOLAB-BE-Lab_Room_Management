using BookLAB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Models
{
    public class ScheduleDto2
    {
        public Guid Id { get; init; }
        public string SubjectCode { get; init; } = string.Empty;
        public string LecturerName { get; init; } = string.Empty; // From User.FullName
        public string UserCode { get; init; } = string.Empty;
        public string LabRoomName { get; init; } = string.Empty; // From LabRoom.Name
        public string SlotName { get; init; } = string.Empty;    // From SlotType.Name
        public string? GroupName { get; init; }                 // From Group.Name
        public DateTimeOffset StartTime { get; init; }
        public DateTimeOffset EndTime { get; init; }
        public int StudentCount { get; init; }
        public ScheduleStatus Status { get; init; }     // Enum string representation
        public ScheduleType Type { get; init; }    // Enum string representation
        public DateTimeOffset CreatedAt { get; init; }
    }
}
