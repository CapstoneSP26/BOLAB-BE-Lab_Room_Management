using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs.Schedules
{
    public class AutoUpdateScheduleStatusJob
    {
        private readonly IUnitOfWork _unitOfWork;

        public AutoUpdateScheduleStatusJob(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task Execute()
        {
            var now = DateTimeOffset.UtcNow;

            // 1. Chuyển Active -> InProcess (Khi đã đến giờ bắt đầu)
            var startingSchedules = await _unitOfWork.Repository<Schedule>().Entities
                .Where(s => s.ScheduleStatus == ScheduleStatus.Active
                         && s.StartTime <= now
                         && s.EndTime > now)
                .ToListAsync();

            foreach (var s in startingSchedules)
            {
                s.ScheduleStatus = ScheduleStatus.InProcess;
            }

            // 2. Chuyển InProcess (hoặc Active bỏ quên) -> Completed (Khi đã quá giờ kết thúc)
            var endingSchedules = await _unitOfWork.Repository<Schedule>().Entities
                .Where(s => (s.ScheduleStatus == ScheduleStatus.InProcess || s.ScheduleStatus == ScheduleStatus.Active)
                         && s.EndTime <= now)
                .ToListAsync();

            foreach (var s in endingSchedules)
            {
                s.ScheduleStatus = ScheduleStatus.Completed;
            }

            if (startingSchedules.Any() || endingSchedules.Any())
            {
                await _unitOfWork.SaveChangesAsync(CancellationToken.None);
                Console.WriteLine($"[AutoUpdateStatus] Đã kích hoạt {startingSchedules.Count} và kết thúc {endingSchedules.Count} buổi học.");
            }
        }
    }
}