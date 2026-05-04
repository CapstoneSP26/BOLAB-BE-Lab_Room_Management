using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Profile.DTOs;
using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.Profile.Commands.UpdateNotificationPreferences;

public class UpdateNotificationPreferencesCommandHandler : IRequestHandler<UpdateNotificationPreferencesCommand, NotificationPreferencesDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public UpdateNotificationPreferencesCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<NotificationPreferencesDto> Handle(UpdateNotificationPreferencesCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId
            ?? throw new BusinessException("User is not authenticated.");

        var preferenceRepository = _unitOfWork.Repository<UserNotificationPreference>();
        var preference = await preferenceRepository.GetByIdAsync(currentUserId);

        if (preference is null)
        {
            preference = new UserNotificationPreference
            {
                UserId = currentUserId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await preferenceRepository.AddAsync(preference);
        }

        preference.EmailNotifications = request.EmailNotifications;
        preference.PushNotifications = request.PushNotifications;
        preference.BookingApproved = request.BookingApproved;
        preference.BookingRejected = request.BookingRejected;
        preference.BookingReminder = request.BookingReminder;
        preference.UpdatedAt = DateTimeOffset.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new NotificationPreferencesDto
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
}
