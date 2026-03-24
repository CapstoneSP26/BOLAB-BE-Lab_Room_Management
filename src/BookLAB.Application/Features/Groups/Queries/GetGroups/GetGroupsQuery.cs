using BookLAB.Application.Features.Groups.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Groups.Queries.GetGroups
{
    public record GetGroupsQuery : IRequest<List<GroupDto>>
    {
    }
}
