using BookLAB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory
{
    public class BookingLabManager
    {
        public Guid Id { get; set; }
        public int LabRoomId { get; set; }
        public string BuildingName { get; set; }
        public Guid BookedByUserId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public string PurposeTypeName { get; set; }
        public string Reason { get; set; } = string.Empty;
        public BookingStatus BookingStatus { get; set; }
        public BookingType BookingType { get; set; }
        public int StudentCount { get; set; }
        public int Recur { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
