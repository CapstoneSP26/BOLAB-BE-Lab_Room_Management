using BookLAB.Application.Features.Profile.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Profile.Queries.GetRecentActivities;

public class GetRecentActivitiesQuery : IRequest<List<RecentActivityDto>>
{
    public int Limit { get; set; } = 10;
}
