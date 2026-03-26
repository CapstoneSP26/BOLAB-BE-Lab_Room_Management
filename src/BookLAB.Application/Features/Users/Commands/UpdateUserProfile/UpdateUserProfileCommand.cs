using BookLAB.Application.Common.Models;
using MediatR;

namespace BookLAB.Application.Features.Users.Commands.UpdateUserProfile
{
    public record UpdateUserProfileCommand : IRequest<UserProfileDto>
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string UserImageUrl { get; set; } = string.Empty;
    }
}
