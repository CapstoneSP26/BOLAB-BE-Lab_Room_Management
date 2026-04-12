using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs.Schedules
{
    public class CreateScheduleJob
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBackgroundJobService _jobService;

        public CreateScheduleJob(IUnitOfWork unitOfWork, IBackgroundJobService jobService)
        {
            _unitOfWork = unitOfWork;
            _jobService = jobService;
        }

        public async Task Execute(Guid bookingId)
        {
            var booking = await _unitOfWork.Repository<Booking>()
                .GetByIdAsync(bookingId);

            if (booking == null) return;

            // 👉 Idempotent check
            var exists = await _unitOfWork.Repository<Schedule>().Entities
                .AnyAsync(s => s.BookingId == bookingId);

            if (exists) return;

            var schedule = new Schedule
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                LabRoomId = booking.LabRoomId,
                LecturerId = booking.CreatedBy ?? Guid.Empty,
                SlotTypeId = booking.SlotTypeId,
                StudentCount = booking.StudentCount,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                ScheduleStatus = ScheduleStatus.Active,
                ScheduleType = ScheduleType.Personal,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = booking.CreatedBy
            };

            await _unitOfWork.Repository<Schedule>().AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

            // 5. SAU KHI LƯU THÀNH CÔNG MỚI ĐẶT LỊCH REMINDER
            var reminderTime = schedule.StartTime.AddMinutes(-30);
            var now = DateTimeOffset.UtcNow;

            if (reminderTime > now)
            {
                var delay = reminderTime - now;

                // Truyền ID đã chắc chắn có trong DB vào Job
                _jobService.Schedule<LecturerReminderJob>(
                    x => x.Execute(schedule.Id),
                    delay);

                // Log để debug trên Hangfire Dashboard dễ hơn
                Console.WriteLine($"[ScheduleJob] Đã đặt lịch nhắc nhở cho Schedule {schedule.Id} vào lúc {reminderTime}");
            }
        }
    }
}
