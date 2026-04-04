using BookLAB.Domain.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Dashboard.Queries.GetDashboardStats
{
    public class GetDashboardStatsQuery : IRequest<DashboardStatsResponseDTO>
    {
    }
}
