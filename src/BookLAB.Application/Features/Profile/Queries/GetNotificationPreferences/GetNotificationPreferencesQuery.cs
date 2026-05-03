using BookLAB.Application.Features.Profile.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Profile.Queries.GetNotificationPreferences;

public class GetNotificationPreferencesQuery : IRequest<NotificationPreferencesDto>
{
}
