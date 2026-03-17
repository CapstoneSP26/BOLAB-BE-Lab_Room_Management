using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Features.Attendances.Commands.SubmitTraditionalAttendance
{
    public class SubmitAttendanceHandler : IRequestHandler<SubmitAttendanceCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public SubmitAttendanceHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<bool> Handle(SubmitAttendanceCommand request, CancellationToken ct)
        {
            var attendanceRepo = _unitOfWork.Repository<Attendance>();
            var now = DateTimeOffset.UtcNow;
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;
            //SECURITY CHECK: Verify if the current user is the lecturer assigned to this schedule
            // Use AsNoTracking for a fast existence and ownership check
            var isAssignedLecturer = await _unitOfWork.Repository<Schedule>().Entities
                .AsNoTracking()
                .AnyAsync(s => s.Id == request.ScheduleId && s.LecturerId == currentUserId, ct);

            if (!isAssignedLecturer)
            {
                // Throwing an exception or returning false depending on your global exception handling strategy
                // Typically, a Forbidden access exception is preferred here
                throw new UnauthorizedAccessException("You are not authorized to take attendance for this schedule.");
            }
            // 1. Identify users who already have an attendance record for this schedule
            // Using AsNoTracking for performance since we only need the IDs
            var existingUserIds = await attendanceRepo.Entities
                .AsNoTracking()
                .Where(a => a.ScheduleId == request.ScheduleId)
                .Select(a => a.UserId)
                .ToListAsync(ct);

            // 2. Separate incoming data into 'Update' and 'Insert' groups
            var updateItems = request.AttendanceItems.Where(i => existingUserIds.Contains(i.UserId)).ToList();
            var insertItems = request.AttendanceItems.Where(i => !existingUserIds.Contains(i.UserId)).ToList();

            // 3. BULK UPDATE: Using EF Core ExecuteUpdateAsync for high performance
            // This executes a single SQL command for each unique status in the batch
            if (updateItems.Any())
            {
                foreach (var statusGroup in updateItems.GroupBy(x => x.Status))
                {
                    var targetUserIds = statusGroup.Select(x => x.UserId).ToList();
                    var status = statusGroup.Key;

                    await attendanceRepo.Entities
                        .Where(a => a.ScheduleId == request.ScheduleId && targetUserIds.Contains(a.UserId))
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(a => a.AttendanceStatus, status)
                            .SetProperty(a => a.CheckInTime, status == AttendanceStatus.Present ? now : null)
                            .SetProperty(a => a.UpdatedAt, now)
                            .SetProperty(a => a.UpdatedBy, currentUserId), ct); // Assuming UoW tracks current user
                }
            }

            // 4. BULK INSERT: Use AddRangeAsync for new records
            if (insertItems.Any())
            {
                var newAttendances = insertItems.Select(item => new Attendance
                {
                    ScheduleId = request.ScheduleId,
                    UserId = item.UserId,
                    AttendanceStatus = item.Status,
                    CheckInMethod = AttendanceCheckInMethod.Manual,
                    CheckInTime = item.Status == AttendanceStatus.Present ? now : null,
                    CreatedAt = now,
                    CreatedBy = currentUserId,
                }).ToList();

                await attendanceRepo.AddRangeAsync(newAttendances);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            return true;
        }
    }
}
