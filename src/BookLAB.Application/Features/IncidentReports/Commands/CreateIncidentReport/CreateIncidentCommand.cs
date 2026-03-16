using System.Text.Json.Serialization;
using BookLAB.Domain.Enums;
using MediatR;

namespace BookLAB.Application.Features.IncidentReports.Commands.CreateIncidentReport
{
    public record CreateIncidentCommand : IRequest<CreateIncidentResponse>
    {
        public Guid ReportedBy { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public IncidentSeverity Severity { get; init; }

        public string? Environment { get; init; }
        public List<string> StepsToReproduce { get; init; } = new();
        public string? ExpectedResult { get; init; }
        public string? ActualResult { get; init; }
        public string? AttachmentUrl { get; init; }
    }
}