using MediatR;

namespace BookLAB.Application.Features.Groups.Queries.GetGroupById
{
    public record GetGroupByIdQuery : IRequest<GroupDetailDto>
    {
        public Guid GroupId { get; init; }
    }

    public class GroupDetailDto
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
