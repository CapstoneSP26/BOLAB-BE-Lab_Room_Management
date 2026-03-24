using BookLAB.Domain.Enums;
using Google.Apis.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Feedbacks.Queries.GetReportedReport
{
    public class ReportResponseDto
    {
        public Guid Id { get; set; }
        public Guid ReportId { get; set; }
        public int LabRoomId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Severity { get; set; }
        public string Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? ResolvedBy { get; set; }

    }
}
