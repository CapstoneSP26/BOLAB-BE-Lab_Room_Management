
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Jobs.Schedules;
using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Commands.ImportSchedule
{
    public class ConfirmFlexibleImportHandler : IRequestHandler<ConfirmFlexibleImportCommand, ImportResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleImportService _scheduleImportService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBackgroundJobService _jobService;

        public ConfirmFlexibleImportHandler(IUnitOfWork unitOfWork, IScheduleImportService scheduleImportService, ICurrentUserService currentUserService, IBackgroundJobService jobService)
        {
            _unitOfWork = unitOfWork;
            _scheduleImportService = scheduleImportService;
            _currentUserService = currentUserService;
            _jobService = jobService;
        }

        public async Task<ImportResult> Handle(ConfirmFlexibleImportCommand request, CancellationToken cancellationToken)
        {
            var result = await _scheduleImportService.ValidateFlexibleAsync(request.Schedules, request.CampusId, request.StartTime, request.EndTime, cancellationToken, true);
            var countUpdated = result.Rows.Count(r => r.Data.IsUpdated);
            var countNew = result.Rows.Count(r => !r.Data.IsUpdated);
            var now = DateTimeOffset.UtcNow;
            if (!result.CanCommit)
            {
                return new ImportResult
                {
                    Success = false,
                };
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var newSchedules = new List<Schedule>();
                foreach (var row in result.Rows)
                {
                    var entity = row.ConvertedEntity;
                    if (row.Data.IsUpdated)
                    {
                        entity.UpdatedAt = now;
                        entity.UpdatedBy = _currentUserService.UserId;
                        _unitOfWork.Repository<Schedule>().Update(entity);
                    }
                    else
                    {
                        entity.ScheduleStatus = ScheduleStatus.Active;
                        entity.ScheduleType = ScheduleType.Academic;
                        entity.CreatedAt = now;
                        entity.CreatedBy = _currentUserService.UserId;
                        newSchedules.Add(entity);
                    }                
                }
                if (newSchedules.Any())
                {
                    await _unitOfWork.Repository<Schedule>().AddRangeAsync(newSchedules);
                }
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync();

                foreach (var schedule in newSchedules)
                {
                    var reminderTime = schedule.StartTime.AddMinutes(-30);
                    // Chỉ đặt lịch nếu thời điểm nhắc nhở vẫn còn ở tương lai
                    if (reminderTime > DateTimeOffset.UtcNow)
                    {
                        var delay = reminderTime - DateTimeOffset.UtcNow;

                        _jobService.Schedule<LecturerReminderJob>(
                            x => x.Execute(schedule.Id),
                            delay);
                    }
                }

                return new ImportResult
                {
                    Success = true,
                    CreatedCount = countNew,
                    UpdatedCount = countUpdated,
                    TotalProcessed = result.Rows.Count,
                };
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }

        }
    }
}
