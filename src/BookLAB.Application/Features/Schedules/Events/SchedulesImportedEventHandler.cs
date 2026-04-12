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
        private readonly IUnitOfWork _unitOfWork;

        public SchedulesImportedEventHandler(IBackgroundJobService jobService, IUnitOfWork unitOfWork)
        {
            _jobService = jobService;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(SchedulesImportedEvent notification, CancellationToken cancellationToken)
        {
            foreach (var scheduleId in notification.ScheduleIds)
            {
                // 1. Đẩy job nhắc nhở giảng viên (30p trước giờ dạy)
                // Lưu ý: Job này sẽ tự lấy StartTime trong DB nên không cần truyền thêm dữ liệu
                _jobService.Enqueue<PrepareLecturerReminderJob>(x => x.Execute(scheduleId));

                // 2. Đẩy job gửi mail cho toàn bộ sinh viên trong lớp
                _jobService.Enqueue<StudentScheduleNotifyJob>(x => x.Execute(scheduleId));
            }

            await Task.CompletedTask;
        }
    }
}
