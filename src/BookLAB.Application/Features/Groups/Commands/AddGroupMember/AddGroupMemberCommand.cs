using MediatR;

namespace BookLAB.Application.Features.Groups.Commands.AddGroupMember
{
    public record AddGroupMemberCommand : IRequest
    {
        public Guid GroupId { get; init; }
        public Guid UserId { get; init; }
        public string SubjectCode { get; init; } = string.Empty;
    }
}
