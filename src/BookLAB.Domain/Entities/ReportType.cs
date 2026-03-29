using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Domain.Entities
{
    public class ReportType
    {
        public int ReportTypeId { get; set; }
        public string ReportTypeName { get; set; }
        public ICollection<Report> Reports = new List<Report>();
    }
}
