using MediatR;

namespace BookLAB.Application.Features.IncidentReports.Queries.GetUnresolvedIncidents
{
    public class GetUnresolvedIncidentsQuery : IRequest<List<IncidentDto>>
    {
        public int? Limit { get; set; } = 10;
    }
}
