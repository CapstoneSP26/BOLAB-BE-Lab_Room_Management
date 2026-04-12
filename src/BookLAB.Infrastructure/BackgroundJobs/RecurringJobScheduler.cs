using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Jobs.Bookings;
using BookLAB.Application.Common.Jobs.Schedules;
using Hangfire;

namespace BookLAB.Infrastructure.BackgroundJobs
{
    public class RecurringJobScheduler
    {
        private readonly IBackgroundJobService _jobService;

        public RecurringJobScheduler(IBackgroundJobService jobService)
        {
            _jobService = jobService;
        }

        public void Schedule()
        {
            _jobService.AddOrUpdateRecurring<AutoRejectBookingJob>(
                "auto-reject-bookings",
                job => job.Execute(),
                Cron.MinuteInterval(5) // hoặc lấy từ config
            );

            // Quét mỗi 5 phút một lần để đảm bảo độ trễ thấp
            _jobService.AddOrUpdateRecurring<AutoUpdateScheduleStatusJob>(
                "auto-update-schedule-status",
                job => job.Execute(),
                Cron.MinuteInterval(5) // Cron expression cho mỗi 5 phút
            );
        }
    }
}