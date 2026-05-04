using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Models
{
    public class ReportRequestDto
    {
        public string? Q { get; set; }
        public int? BuildingId { get; set; }
        public int? RoomId { get; set; }
        public string? ReportType { get; set; }
        public bool? IsResolved { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? Page { get; set; }
        public int? Limit { get; set; }
        public string? SortBy { get; set; }
        public bool? IsDescending { get; set; }
    }
}
