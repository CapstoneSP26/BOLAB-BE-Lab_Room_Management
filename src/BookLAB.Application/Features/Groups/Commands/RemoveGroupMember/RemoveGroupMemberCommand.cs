using MediatR;

namespace BookLAB.Application.Features.Groups.Commands.RemoveGroupMember
{
    public record RemoveGroupMemberCommand : IRequest
    {
        public Guid GroupId { get; init; }
        public Guid UserId { get; init; }
    }
}
