using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Interfaces.Jobs;
using BookLAB.Application.Common.Jobs.Schedules;

public class ScheduleImportPostProcessingJob
{
    private readonly IBackgroundJobService _jobService;

    public ScheduleImportPostProcessingJob(IBackgroundJobService jobService)
    {
        _jobService = jobService;
    }

    public async Task Execute(List<Guid> scheduleIds)
    {
        if (scheduleIds == null || !scheduleIds.Any()) return;

        foreach (var id in scheduleIds)
        {
            // Chạy ngầm hoàn toàn, không gây block UI
            _jobService.Enqueue<PrepareLecturerReminderJob>(x => x.Execute(id));
            _jobService.Enqueue<IStudentScheduleNotifyJob>(x => x.Execute(id));
        }

        await Task.CompletedTask;
    }
}