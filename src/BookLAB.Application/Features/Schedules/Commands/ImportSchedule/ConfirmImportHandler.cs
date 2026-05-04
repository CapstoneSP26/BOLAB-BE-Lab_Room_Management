using BookLAB.Application.Common.Extensions;
using BookLAB.Application.Common.Interfaces.Identity;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Schedules.Events;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics; // Thêm thư viện này

namespace BookLAB.Application.Features.Schedules.Commands.ImportSchedule
{
    public class ConfirmImportHandler : IRequestHandler<ConfirmImportCommand, ImportResult>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleImportService _scheduleImportService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMediator _mediator;
        private readonly ILogger<ConfirmImportHandler> _logger; // Nên inject thêm Logger


        public ConfirmImportHandler(IUnitOfWork unitOfWork, IScheduleImportService scheduleImportService, ICurrentUserService currentUserService, IBackgroundJobService jobService, IMediator mediator, ILogger<ConfirmImportHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _scheduleImportService = scheduleImportService;
            _currentUserService = currentUserService;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<ImportResult> Handle(ConfirmImportCommand request, CancellationToken cancellationToken)
        {
            var totalSw = Stopwatch.StartNew();
            var stepSw = new Stopwatch();

            _logger.LogInformation("--- Bắt đầu Import Schedule cho Batch: {BatchName} ---", request.BatchName);

            // 1. Track phần Validate
            stepSw.Start();
            var response = await _scheduleImportService.ValidateAsync(request.Schedules, request.CampusId, request.StartTime, request.EndTime, request.ImportBatchId, cancellationToken, true);
            stepSw.Stop();
            _logger.LogInformation("[TIMER] 1. ValidateAsync: {Elapsed}ms", stepSw.ElapsedMilliseconds);
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

            stepSw.Restart();
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (request.ImportBatchId != null && maps.ImportBatch == null)
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
                var updatedIds = new HashSet<Guid>();
                foreach (var row in result.Rows)
                {
                    var entity = row.ConvertedEntity;
                    if (row.Data.IsUpdated)
                    {
                        updatedIds.Add(entity.Id);
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
            
                if (request.ImportBatchId != null)
                {
                    var deleteScheduleBatch = maps.ExistingSchedules.Where(s => s.ImportBatchId == request.ImportBatchId && !updatedIds.Contains(s.Id)).ToList();
                    if (deleteScheduleBatch.Any())
                    {
                        _unitOfWork.Repository<Schedule>().DeleteRange(deleteScheduleBatch);
                    }
                }
                stepSw.Stop();
                _logger.LogInformation("[TIMER] 2. Prepare Data & AddRange: {Elapsed}ms", stepSw.ElapsedMilliseconds);

                stepSw.Restart();
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                stepSw.Stop();
                _logger.LogInformation("[TIMER] 3. SaveChangesAsync: {Elapsed}ms", stepSw.ElapsedMilliseconds);

                stepSw.Restart();
                await _unitOfWork.CommitTransactionAsync();
                stepSw.Stop();
                _logger.LogInformation("[TIMER] 4. CommitTransaction: {Elapsed}ms", stepSw.ElapsedMilliseconds);

                var scheduleIds = newSchedules.Select(s => s.Id).ToList();

                stepSw.Restart();
                if (scheduleIds.Any())
                {
                    await _mediator.Publish(new SchedulesImportedEvent(scheduleIds), cancellationToken);
                }
                stepSw.Stop();
                _logger.LogInformation("[TIMER] 5. Mediator Publish Event: {Elapsed}ms", stepSw.ElapsedMilliseconds);

                totalSw.Stop();
                _logger.LogInformation("--- Hoàn tất Import. Tổng thời gian: {Elapsed}ms ---", totalSw.ElapsedMilliseconds);

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
