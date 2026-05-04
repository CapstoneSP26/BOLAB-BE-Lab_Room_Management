using BookLAB.Application.Features.Profile.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Profile.Queries.GetMyProfile;

public class GetMyProfileQuery : IRequest<MyProfileDto>
{
}
