using MediatR;

namespace BookLAB.Application.Features.Profile.Commands.UpdateAvatar;

public class UpdateAvatarCommand : IRequest<string>
{
    public dynamic Avatar { get; set; } = default!;
}
