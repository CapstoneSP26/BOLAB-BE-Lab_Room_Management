using BookLAB.Application.Features.Groups.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Groups.Queries.GetGroupById
{
    public record GetGroupByIdQuery : IRequest<GroupDto>
    {
        public Guid GroupId { get; init; }
    }
}
