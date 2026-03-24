using BookLAB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Models
{
    public class ReportDto
    {
        public Guid ScheduleId { get; set; }
        public ReportType ReportType { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsResolved { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
