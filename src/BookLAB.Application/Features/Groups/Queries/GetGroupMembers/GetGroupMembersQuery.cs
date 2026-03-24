using MediatR;

namespace BookLAB.Application.Features.Groups.Queries.GetGroupMembers
{
    public record GetGroupMembersQuery : IRequest<List<GroupMemberDto>>
    {
        public Guid GroupId { get; init; }
    }

    public class GroupMemberDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserCode { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
    }
}
