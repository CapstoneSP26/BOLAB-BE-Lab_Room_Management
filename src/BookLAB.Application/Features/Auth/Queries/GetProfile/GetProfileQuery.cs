using BookLAB.Application.Common.Models;
using MediatR;

namespace BookLAB.Application.Features.Auth.Queries.GetProfile
{
    public record GetProfileQuery : IRequest<UserProfileDto>;
}
