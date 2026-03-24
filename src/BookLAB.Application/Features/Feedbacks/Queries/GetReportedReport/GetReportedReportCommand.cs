using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.Feedbacks.Queries.GetReportedReport
{
    public class GetReportedReportCommand : IRequest<List<ReportResponseDto>>
    {
        public Guid userId { get; set; }
    }
}
