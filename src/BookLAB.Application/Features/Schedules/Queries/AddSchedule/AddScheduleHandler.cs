using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
namespace BookLAB.Application.Features.Schedules.Queries.AddSchedule
{
    public class AddScheduleHandler : IRequestHandler<AddScheduleCommand, bool>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IScheduleService _scheduleService;
        private readonly ILogger<AddScheduleHandler> _logger;

        public AddScheduleHandler(IUnitOfWork unitOfWork,
            IScheduleService scheduleService,
            ILogger<AddScheduleHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _scheduleService = scheduleService;
            _logger = logger;
        }

        public async Task<bool> Handle(AddScheduleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if the new schedule conflicts with existing schedules
                if (await _scheduleService.CheckConflictAsync(
                    request.Schedule.LabRoomId,
                    request.Schedule.StartTime,
                    request.Schedule.EndTime,
                    cancellationToken))
                    return false; // If there is a conflict, return false and do not add the schedule

                // Begin a database transaction to ensure data consistency
                await _unitOfWork.BeginTransactionAsync();

                // Add the new schedule to the repository
                await _unitOfWork.Repository<Schedule>().AddAsync(request.Schedule);

                // Save changes to the database
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                // Commit the transaction if everything succeeds
                await _unitOfWork.CommitTransactionAsync();

                return true; // Return true if the schedule was added successfully
            }
            catch (Exception ex)
            {
                // Roll back the transaction if an error occurs
                await _unitOfWork.RollbackTransactionAsync();

                // Log the error with details for debugging
                _logger.LogError(ex, $"An error occurred while adding a schedule: {ex.Message}");

                return false; // Return false if an exception was thrown
            }
        }

    }
}
