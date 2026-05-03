namespace BookLAB.Application.Features.Profile.DTOs;

public class NotificationPreferencesDto
{
    public Guid UserId { get; set; }

    public bool EmailNotifications { get; set; }

    public bool PushNotifications { get; set; }

    public bool BookingApproved { get; set; }

    public bool BookingRejected { get; set; }

    public bool BookingReminder { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
