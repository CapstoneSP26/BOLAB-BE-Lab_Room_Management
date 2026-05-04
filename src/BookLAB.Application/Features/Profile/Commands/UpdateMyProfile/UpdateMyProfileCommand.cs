using BookLAB.Application.Features.Profile.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Profile.Commands.UpdateMyProfile;

public class UpdateMyProfileCommand : IRequest<MyProfileDto>
{
    public string? FullName { get; set; }
    
    public string? Email { get; set; }
}
