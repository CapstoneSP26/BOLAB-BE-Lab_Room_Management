using MediatR;

namespace BookLAB.Application.Features.Groups.Commands.UpdateGroupMember
{
    public record UpdateGroupMemberCommand : IRequest
    {
        public Guid GroupId { get; init; }
        public Guid UserId { get; init; }
        public string SubjectCode { get; init; } = string.Empty;
    }
}
