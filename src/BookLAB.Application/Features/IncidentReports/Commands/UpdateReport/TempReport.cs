using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.IncidentReports.Commands.UpdateReport
{
    public class TempReport
    {
        public Guid Id { get; set; }
        public Guid ScheduleId { get; set; }
        public int? ReportTypeId { get; set; }
        public string? Description { get; set; } = string.Empty;
        public bool? IsResolved { get; set; } = false;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
