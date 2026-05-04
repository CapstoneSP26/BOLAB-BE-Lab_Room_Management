using BookLAB.Application.Features.Profile.DTOs;
using MediatR;

namespace BookLAB.Application.Features.Profile.Commands.UpdateNotificationPreferences;

public class UpdateNotificationPreferencesRequest : IRequest<NotificationPreferencesDto>
{
    public bool EmailNotifications { get; set; }

    public bool PushNotifications { get; set; }

    public bool BookingApproved { get; set; }

    public bool BookingRejected { get; set; }

    public bool BookingReminder { get; set; }
}
