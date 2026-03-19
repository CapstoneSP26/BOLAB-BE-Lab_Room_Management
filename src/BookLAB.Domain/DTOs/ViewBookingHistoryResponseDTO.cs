using BookLAB.Domain.Entities;
using Google.Apis.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Domain.DTOs
{
    public class ViewBookingHistoryResponseDTO
    {
        public string id { get; set; }
        public string roomId { get; set; }
        public string roomName { get; set; }
        public string buildingName { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string date { get; set; }
        public string status { get; set; }
        public string purpose { get; set; }
        public string userName { get; set; }
    }
}
