using MediatR;

namespace BookLAB.Application.Features.Groups.Commands.DeleteGroup
{
    public record DeleteGroupCommand : IRequest
    {
        public Guid GroupId { get; init; }
    }
}
