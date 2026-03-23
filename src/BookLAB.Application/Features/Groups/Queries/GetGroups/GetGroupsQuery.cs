using MediatR;

namespace BookLAB.Application.Features.Groups.Queries.GetGroups
{
    public record GetGroupsQuery : IRequest<List<GroupDto>>
    {
    }

    public class GroupDto
    {
        public Guid Id { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public int MembersCount { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
