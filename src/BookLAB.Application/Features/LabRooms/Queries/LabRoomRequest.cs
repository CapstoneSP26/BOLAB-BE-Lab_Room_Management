using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.LabRooms.Queries
{
    public class LabRoomRequest
    {
        public int id { get; set; }
        public string name { get; set; }
        public int building { get; set; }
        public int capacity { get; set; }
        public string[]? features { get; set; }
        public string? image { get; set; }
    }
}
