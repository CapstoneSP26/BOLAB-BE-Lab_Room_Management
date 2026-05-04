using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Profile.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Profile.Queries.GetNotificationPreferences;

public class GetNotificationPreferencesQueryHandler : IRequestHandler<GetNotificationPreferencesQuery, NotificationPreferencesDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetNotificationPreferencesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<NotificationPreferencesDto> Handle(GetNotificationPreferencesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId
            ?? throw new BusinessException("User is not authenticated.");

        var preference = await _unitOfWork.Repository<UserNotificationPreference>().Entities
            .FirstOrDefaultAsync(x => x.UserId == currentUserId, cancellationToken);

        if (preference is null)
        {
            preference = CreateDefaultPreference(currentUserId);
            await _unitOfWork.Repository<UserNotificationPreference>().AddAsync(preference);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return Map(preference);
    }

    private static UserNotificationPreference CreateDefaultPreference(Guid userId) => new()
    {
        UserId = userId,
        EmailNotifications = true,
        PushNotifications = true,
        BookingApproved = true,
        BookingRejected = true,
        BookingReminder = true,
        CreatedAt = DateTimeOffset.UtcNow
    };

    private static NotificationPreferencesDto Map(UserNotificationPreference preference) => new()
    {
        UserId = preference.UserId,
        EmailNotifications = preference.EmailNotifications,
        PushNotifications = preference.PushNotifications,
        BookingApproved = preference.BookingApproved,
        BookingRejected = preference.BookingRejected,
        BookingReminder = preference.BookingReminder,
        CreatedAt = preference.CreatedAt,
        UpdatedAt = preference.UpdatedAt
    };
}
