using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Domain.DTOs
{
    /// <summary>
    /// Only need userId
    /// </summary>
    public class ViewBookingHistoryDTO
    {
        public int page { get; set; } = 1;
        public int limit { get; set; } = 10;
        public string status { get; set; } = "all";
        public DateTimeOffset startDate { get; set; } = DateTimeOffset.Parse("2006-01-01T00:00:00Z");
        public DateTimeOffset endDate { get; set; } = DateTimeOffset.Parse("9999-01-01T00:00:00Z");
        public int? labRoomId { get; set; }
    }
}
