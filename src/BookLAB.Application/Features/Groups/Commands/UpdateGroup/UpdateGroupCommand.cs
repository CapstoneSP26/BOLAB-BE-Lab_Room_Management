using MediatR;

namespace BookLAB.Application.Features.Groups.Commands.UpdateGroup
{
    public record UpdateGroupCommand : IRequest
    {
        public Guid GroupId { get; init; }
        public string GroupName { get; init; } = string.Empty;
        public string? Description { get; init; }
    }
}
