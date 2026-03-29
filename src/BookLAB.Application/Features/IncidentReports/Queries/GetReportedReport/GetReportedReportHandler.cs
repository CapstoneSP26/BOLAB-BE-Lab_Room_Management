using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Features.IncidentReports.Queries.GetReportedReport
{
    public class GetReportedReportHandler : IRequestHandler<GetReportedReportCommand, List<ReportResponseDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GetReportedReportHandler> _logger;

        public GetReportedReportHandler(IUnitOfWork unitOfWork, ILogger<GetReportedReportHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<List<ReportResponseDto>> Handle(GetReportedReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _unitOfWork.Repository<Report>().Entities
                    .Include(x => x.Schedule.LabRoom)
                    .Include(x => x.ReportType)
                    .Where(x => x.UpdatedBy == request.userId && x.IsResolved == true).ToListAsync();
                List<ReportResponseDto> results = new List<ReportResponseDto>();

                foreach (var resultDto in result)
                {
                    results.Add(new ReportResponseDto
                    {
                        Id = resultDto.Id,
                        ReportId = resultDto.Id,
                        LabRoomId = resultDto.Schedule.LabRoom.Id,
                        Title = resultDto.ReportType?.ReportTypeName ?? "Unknown",
                        Severity = "HIGH",
                        Status = "RESOLVED",
                        ResolvedAt = resultDto.UpdatedAt,
                        ResolvedBy = resultDto.UpdatedBy,
                        CreatedAt = resultDto.CreatedAt,
                    });
                }

                return results;
            } catch (Exception ex)
            {
                return new List<ReportResponseDto>();
            }
        }
    }
}
