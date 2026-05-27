using BookLAB.Application.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Dashboard.Queries.GetPdfStatistic
{
    public class GetPdfStatisticQueryReturn
    {
        public int TotalLabRoom { get; set; }
        public List<LabRoomDto> LabRooms { get; set; } = new List<LabRoomDto>();
        public string TimeType { get; set; } = string.Empty;
        public List<LabRoomExtraInfo> LabRoomExtraInfos { get; set; } = new List<LabRoomExtraInfo>();
        public int TotalApprovedBookings { get; set; }
        public int TotalApprovedBookingsBefore { get; set; }
        public double BookingFrequencyByNewSlot { get; set; }
        public double BookingFrequencyByOldSlot { get; set; }
        public double BookingFrequencyByFlexibleSlot { get; set; }
        public double BookingFrequencyByAllSlot { get; set; }
        public int RejectedBookings { get; set; }
        public double AverageBookingsPerTime { get; set; }
        public int TotalReportedIncident { get; set; }
    }

    public class LabRoomExtraInfo
    {
        public int LabRoomId { get; set; }
        public byte[]? Chart { get; set; }
        public double? UsageRated { get; set; }
        public string? IncreasedRate { get; set; }
        public int? NumberOfIncidents { get; set; }
        public int? UnresolvedIncidents { get; set; }
        public double? AverageFixingTime { get; set; }
    }
}
