using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Jobs
{
    public class CreateScheduleJob
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateScheduleJob(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
                ScheduleType = ScheduleType.Booking,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = booking.CreatedBy
            };

            await _unitOfWork.Repository<Schedule>().AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        }
    }
}
