using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Profile.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Profile.Queries.GetProfileStatistics;

public class GetProfileStatisticsQueryHandler : IRequestHandler<GetProfileStatisticsQuery, ProfileStatisticsDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetProfileStatisticsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<ProfileStatisticsDto> Handle(GetProfileStatisticsQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId
            ?? throw new BusinessException("User is not authenticated.");

        // Count total bookings by current user
        var totalBookings = await _unitOfWork.Repository<Booking>().Entities
            .Where(b => b.CreatedBy == currentUserId)
            .CountAsync(cancellationToken);

        // Count total managed classes (Groups)
        var totalManagedClasses = await _unitOfWork.Repository<Group>().Entities
            .Where(g => g.CreatedBy == currentUserId && !g.IsDeleted)
            .CountAsync(cancellationToken);

        // Count total hours taught (sum of schedule durations)
        var totalHoursTaught = await _unitOfWork.Repository<Schedule>().Entities
            .Where(s => s.CreatedBy == currentUserId && !s.IsDeleted)
            .SumAsync(s => (int)(s.EndTime - s.StartTime).TotalHours, cancellationToken);

        // Count total students led (distinct users in groups managed by current user)
        var totalStudentsLed = await _unitOfWork.Repository<GroupMember>().Entities
            .Include(gm => gm.Group)
            .Where(gm => gm.Group.CreatedBy == currentUserId && !gm.Group.IsDeleted)
            .Select(gm => gm.UserId)
            .Distinct()
            .CountAsync(cancellationToken);

        // Count total QA sessions (attendances as lecturer)
        var totalQASessions = await _unitOfWork.Repository<Attendance>().Entities
            .Where(a => a.CreatedBy == currentUserId)
            .CountAsync(cancellationToken);

        return new ProfileStatisticsDto
        {
            TotalBookings = totalBookings,
            TotalManagedClasses = totalManagedClasses,
            TotalHoursTaught = totalHoursTaught,
            TotalStudentsLed = totalStudentsLed,
            TotalQASessions = totalQASessions
        };
    }
}
