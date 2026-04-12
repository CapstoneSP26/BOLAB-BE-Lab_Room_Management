using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Events;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Commands.ImportSchedule
{
    public class ConfirmImportHandler : IRequestHandler<ConfirmImportCommand, ImportResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleImportService _scheduleImportService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IBackgroundJobService _jobService;
        private readonly IMediator _mediator;


        public ConfirmImportHandler(IUnitOfWork unitOfWork, IScheduleImportService scheduleImportService, ICurrentUserService currentUserService, IBackgroundJobService jobService, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _scheduleImportService = scheduleImportService;
            _currentUserService = currentUserService;
            _jobService = jobService;
            _mediator = mediator;
        }

        public async Task<ImportResult> Handle(ConfirmImportCommand request, CancellationToken cancellationToken)
        {
            var result = await _scheduleImportService.ValidateAsync(request.Schedules, request.CampusId, request.StartTime, request.EndTime, cancellationToken, true);
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
                    var scheduleIds = newSchedules.Select(s => s.Id).ToList();
                    await _mediator.Publish(new SchedulesImportedEvent(scheduleIds), cancellationToken);
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
