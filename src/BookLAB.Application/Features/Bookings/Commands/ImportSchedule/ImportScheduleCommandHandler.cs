
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Persistence;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Bookings.Commands.ImportSchedule
{
    public class ImportScheduleCommandHandler : IRequestHandler<ImportScheduleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        public ImportScheduleCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }
        public async Task<bool> Handle(ImportScheduleCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId ?? Guid.Empty;
            var schedulesToImport = request.Schedules.Select(s => new Schedule
            {
                LecturerId = currentUserId,
                LabRoomId = s.LabRoomId,
                ScheduleType = s.ScheduleType,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                StudentCount = s.StudentCount,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = currentUserId
            }).ToList();
            await _unitOfWork.Repository<Schedule>().AddRangeAsync(schedulesToImport);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
}
