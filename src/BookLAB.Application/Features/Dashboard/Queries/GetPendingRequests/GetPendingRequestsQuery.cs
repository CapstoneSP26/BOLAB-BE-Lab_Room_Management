using MediatR;
using BookLAB.Application.Common.Models;

namespace BookLAB.Application.Features.Dashboard.Queries.GetPendingRequests
{
    public class GetPendingRequestsQuery : IRequest<List<PendingRequestDto>>
    {
        public int? Limit { get; set; } = 10;
    }
}
