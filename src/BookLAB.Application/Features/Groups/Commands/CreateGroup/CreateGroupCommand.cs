using MediatR;

namespace BookLAB.Application.Features.Groups.Commands.CreateGroup
{
    public record CreateGroupCommand : IRequest<Guid>
    {
        public string GroupName { get; init; } = string.Empty;
        public string? Description { get; init; }
    }
}
