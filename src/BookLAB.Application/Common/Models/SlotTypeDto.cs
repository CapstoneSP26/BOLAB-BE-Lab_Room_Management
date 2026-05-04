using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Models
{
    public class SlotTypeDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public int CampusId { get; set; }
    }
}
