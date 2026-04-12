using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs.Schedules
{
    public class PrepareLecturerReminderJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobService _jobService;

        public PrepareLecturerReminderJob(IUnitOfWork unitOfWork, IBackgroundJobService jobService)
        {
            _unitOfWork = unitOfWork;
            _jobService = jobService;
        }

        public async Task Execute(Guid scheduleId)
        {
            // 1. Lấy thông tin lịch dạy mới nhất từ DB
            var schedule = await _unitOfWork.Repository<Schedule>().Entities
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            // Kiểm tra nếu lịch không tồn tại hoặc đã bị hủy thì không đặt lịch nhắc nhở
            if (schedule == null || schedule.ScheduleStatus == ScheduleStatus.Cancelled)
            {
                return;
            }

            // 2. Tính toán thời điểm nhắc nhở (30 phút trước giờ bắt đầu)
            var reminderTime = schedule.StartTime.AddMinutes(-30);
            var now = DateTimeOffset.UtcNow;

            // 3. Đưa vào hàng đợi Scheduled Job của Hangfire
            if (reminderTime > now)
            {
                var delay = reminderTime - now;

                // Sử dụng ID để gọi đến Job thực thi gửi Email
                _jobService.Schedule<LecturerReminderJob>(
                    x => x.Execute(scheduleId),
                    delay);

                // Lưu ý: Bạn có thể lưu JobId vào Database nếu muốn quản lý việc hủy Job sau này
            }
        }
    }
}
