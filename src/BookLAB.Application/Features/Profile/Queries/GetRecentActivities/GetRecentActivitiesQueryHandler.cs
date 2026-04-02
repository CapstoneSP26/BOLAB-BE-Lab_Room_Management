using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Features.Profile.DTOs;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Profile.Queries.GetRecentActivities;

public class GetRecentActivitiesQueryHandler : IRequestHandler<GetRecentActivitiesQuery, List<RecentActivityDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public GetRecentActivitiesQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
    {
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<List<RecentActivityDto>> Handle(GetRecentActivitiesQuery request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId
            ?? throw new BusinessException("User is not authenticated.");

        var activities = new List<RecentActivityDto>();

        // Get recent bookings
        var bookings = await _unitOfWork.Repository<Booking>().Entities
            .Include(b => b.LabRoom)
            .Where(b => b.CreatedBy == currentUserId)
            .OrderByDescending(b => b.CreatedAt)
            .Take(request.Limit)
            .Select(b => new RecentActivityDto
            {
                Id = b.Id,
                ActivityType = "Booking",
                Title = $"Booked {b.LabRoom.RoomName}",
                Description = $"Booking slot from {b.StartTime:yyyy-MM-dd} to {b.EndTime:yyyy-MM-dd}",
                LabRoomName = b.LabRoom.RoomName,
                Date = b.CreatedAt,
                Status = b.BookingStatus.ToString()
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(bookings);

        // Get recent schedules
        var schedules = await _unitOfWork.Repository<Schedule>().Entities
            .Include(s => s.LabRoom)
            .Where(s => s.CreatedBy == currentUserId && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .Take(request.Limit)
            .Select(s => new RecentActivityDto
            {
                Id = s.Id,
                ActivityType = "Schedule",
                Title = $"Created Schedule for {s.LabRoom.RoomName}",
                Description = $"Schedule from {s.StartTime:yyyy-MM-dd HH:mm} to {s.EndTime:yyyy-MM-dd HH:mm}",
                LabRoomName = s.LabRoom.RoomName,
                Date = s.CreatedAt,
                Status = "Created"
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(schedules);

        // Get recent attendances
        var attendances = await _unitOfWork.Repository<Attendance>().Entities
            .Include(a => a.Schedule)
            .ThenInclude(s => s.LabRoom)
            .Where(a => a.CreatedBy == currentUserId)
            .OrderByDescending(a => a.CreatedAt)
            .Take(request.Limit)
            .Select(a => new RecentActivityDto
            {
                Id = a.Id,
                ActivityType = "Attendance",
                Title = "Recorded Attendance",
                Description = $"Attendance at {a.Schedule.LabRoom.RoomName}",
                LabRoomName = a.Schedule.LabRoom.RoomName,
                Date = a.CreatedAt,
                Status = "Recorded"
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(attendances);

        // Get recent reports
        var reports = await _unitOfWork.Repository<Report>().Entities
            .Include(r => r.Schedule)
            .ThenInclude(s => s.LabRoom)
            .Where(r => r.CreatedBy == currentUserId)
            .OrderByDescending(r => r.CreatedAt)
            .Take(request.Limit)
            .Select(r => new RecentActivityDto
            {
                Id = r.Id,
                ActivityType = "Report",
                Title = "Submitted Report",
                Description = r.Description,
                LabRoomName = r.Schedule.LabRoom.RoomName,
                Date = r.CreatedAt,
                Status = "Submitted"
            })
            .ToListAsync(cancellationToken);

        activities.AddRange(reports);

        // Sort all activities by date and take the limit
        return activities
            .OrderByDescending(a => a.Date)
            .Take(request.Limit)
            .ToList();
    }
}
