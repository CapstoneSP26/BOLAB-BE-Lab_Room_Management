
using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Jobs.Schedules;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Events;
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
        private readonly IMediator _mediator;

        public ConfirmFlexibleImportHandler(IUnitOfWork unitOfWork, IScheduleImportService scheduleImportService, ICurrentUserService currentUserService, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _scheduleImportService = scheduleImportService;
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        public async Task<ImportResult> Handle(ConfirmFlexibleImportCommand request, CancellationToken cancellationToken)
        {
            var response = await _scheduleImportService.ValidateFlexibleAsync(request.Schedules, request.CampusId, request.StartTime, request.EndTime, request.ImportBatchId, cancellationToken, true);
            var result = response.result;
            var maps = response.maps;
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
                if(request.ImportBatchId != null && maps.ImportBatch == null)
                {
                    return new ImportResult
                    {
                        Success = false,
                    };
                }
                var improtBatchId = request.ImportBatchId;
                if (request.BatchName != null && request.ImportBatchId == null)
                {
                    var newImportBatch = new ImportBatch
                    {
                        Id = Guid.NewGuid(),
                        Name = request.BatchName,
                        ImportBatchType = ImportBatchType.FlexibleSchedule,
                        SemesterName = request.StartTime.AddDays(1).ConvertTimeToSemester(),
                        CreatedAt = now,
                        CreatedBy = _currentUserService.UserId,
                    };
                    await _unitOfWork.Repository<ImportBatch>().AddAsync(newImportBatch);
                    improtBatchId = newImportBatch.Id;
                }
                var newSchedules = new List<Schedule>();
                var updateSchedules = new List<Schedule>();
                foreach (var row in result.Rows)
                {
                    var entity = row.ConvertedEntity;
                    if (row.Data.IsUpdated)
                    {
                        updateSchedules.Add(entity);
                    }
                    else
                    {
                        entity.ImportBatchId = improtBatchId;
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

                if(request.ImportBatchId != null)
                {
                    var updateScheduleHashSet = updateSchedules.Select(s => s.Id).ToHashSet();
                    var deleteScheduleBatch = maps.ExistingSchedules.Where(s => s.ImportBatchId == request.ImportBatchId && !updateScheduleHashSet.Contains(s.Id)).ToList();
                    if (deleteScheduleBatch.Any())
                    {
                        _unitOfWork.Repository<Schedule>().DeleteRange(deleteScheduleBatch);
                    }
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
