using MediatR;

namespace BookLAB.Application.Features.Profile.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<Unit>
{
    public string CurrentPassword { get; set; } = default!;
    public string NewPassword { get; set; } = default!;
}
