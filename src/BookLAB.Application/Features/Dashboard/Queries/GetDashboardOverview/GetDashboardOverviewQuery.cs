using BookLAB.Domain.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Dashboard.Queries.GetDashboardOverview
{
    public class GetDashboardOverviewQuery : IRequest<DashboardOverviewDto>
    {
        public Guid? UserId { get; init; }
        public string Role { get; init; } = string.Empty;
    }
}
