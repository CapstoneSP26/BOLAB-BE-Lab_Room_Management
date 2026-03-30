using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.IncidentReports.Commands.UpdateReport
{
    public class UpdateReportCommand : IRequest<bool>
    {
        public Guid ReportId { get; set; }
        public TempReport TempReport { get; set; }
    }
}
