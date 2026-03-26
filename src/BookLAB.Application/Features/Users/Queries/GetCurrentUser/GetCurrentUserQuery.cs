using BookLAB.Application.Common.Models;
using MediatR;

namespace BookLAB.Application.Features.Users.Queries.GetCurrentUser
{
    public record GetCurrentUserQuery(Guid UserId) : IRequest<UserProfileDto>;
}
