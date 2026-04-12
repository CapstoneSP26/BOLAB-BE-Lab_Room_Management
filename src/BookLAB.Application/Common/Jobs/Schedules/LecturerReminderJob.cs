using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs.Schedules
{
    public class LecturerReminderJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public LecturerReminderJob(IUnitOfWork unitOfWork, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task Execute(Guid scheduleId)
        {
            var schedule = await _unitOfWork.Repository<Schedule>().Entities
                .Include(s => s.User)
                .Include(s => s.LabRoom)
                .FirstOrDefaultAsync(s => s.Id == scheduleId);

            // Chỉ gửi nếu lịch vẫn còn Active (không bị hủy trước đó)
            if (schedule == null || schedule.ScheduleStatus != ScheduleStatus.Active) return;

            var message = $"Thầy/Cô {schedule.User.FullName} có lịch dạy tại phòng {schedule.LabRoom.RoomName} vào lúc {schedule.StartTime:HH:mm}.";

            await _emailService.SendEmailAsync(
                schedule.User.Email,
                "Nhắc nhở lịch dạy tại Lab",
                message);

            // Bạn có thể đẩy thêm Notify qua SignalR ở đây
        }
    }
}
