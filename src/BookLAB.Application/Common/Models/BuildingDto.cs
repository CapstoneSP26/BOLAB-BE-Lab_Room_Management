using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Models
{
    public class BuildingDto
    {
        public int Id { get; set; }
        public int CampusId { get; set; }
        public string BuildingName { get; set; }
        public string Description { get; set; }
        public string BuildingImageUrl { get; set; }
    }
}
