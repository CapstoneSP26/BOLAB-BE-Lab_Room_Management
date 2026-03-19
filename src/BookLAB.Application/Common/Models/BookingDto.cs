using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Models
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public int LabRoomId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public int SlotTypeId { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public BookingType BookingType { get; set; }
        public int StudentCount { get; set; }
        public int Recur { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int PurposeTypeId { get; set; }
        public Guid? ScheduleId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public User User { get; set; }
    }
}
