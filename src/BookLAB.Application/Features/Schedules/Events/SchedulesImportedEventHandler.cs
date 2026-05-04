using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Jobs.Emails;
using BookLAB.Application.Common.Jobs.Schedules;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Events
{
    public class SchedulesImportedEventHandler : INotificationHandler<SchedulesImportedEvent>
    {
        private readonly IBackgroundJobService _jobService;

        public SchedulesImportedEventHandler(IBackgroundJobService jobService, IUnitOfWork unitOfWork)
        {
            _jobService = jobService;
        }

        public async Task Handle(SchedulesImportedEvent notification, CancellationToken cancellationToken)
        {
            _jobService.Enqueue<ScheduleImportPostProcessingJob>(x =>
                x.Execute(notification.ScheduleIds));

            await Task.CompletedTask;
        }
    }
}
