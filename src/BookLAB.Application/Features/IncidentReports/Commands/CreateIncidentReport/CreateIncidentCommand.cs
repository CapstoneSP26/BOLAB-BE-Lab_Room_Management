using System.Text.Json.Serialization;
using MediatR;

namespace BookLAB.Application.Features.IncidentReports.Commands.CreateIncidentReport
{
    public record CreateIncidentCommand : IRequest<CreateIncidentResponse>
    {
        public Guid ScheduleId { get; init; }
        public Guid CreatedBy { get; init; }
        public int ReportTypeId { get; set; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
    }
}