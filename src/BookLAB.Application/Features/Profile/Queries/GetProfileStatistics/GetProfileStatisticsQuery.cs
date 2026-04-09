using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Profile.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Profile.Queries.GetProfileStatistics;

public class GetProfileStatisticsQuery : IRequest<ProfileStatisticsDto>
{
}
