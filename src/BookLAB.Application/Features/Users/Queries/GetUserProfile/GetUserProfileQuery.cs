using MediatR;

namespace BookLAB.Application.Features.Users.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<UserProfileDto>
    {
    }
}
