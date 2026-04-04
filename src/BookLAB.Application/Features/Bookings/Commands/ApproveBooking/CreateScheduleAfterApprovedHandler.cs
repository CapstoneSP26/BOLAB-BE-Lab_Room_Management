using BookLAB.Application.Common.Events;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using MediatR;

namespace BookLAB.Application.Features.Bookings.Commands.ApproveBooking
{
    public class CreateScheduleAfterApprovedHandler : INotificationHandler<BookingApprovedEvent>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateScheduleAfterApprovedHandler(IUnitOfWork unitOfWork)
            => _unitOfWork = unitOfWork;

        public async Task Handle(BookingApprovedEvent notification, CancellationToken cancellationToken)
        {
            var booking = await _unitOfWork.Repository<Booking>().GetByIdAsync(notification.BookingId);
            if (booking == null) return;

            var schedule = new Schedule
            {
                Id = Guid.NewGuid(),
                BookingId = booking.Id,
                SlotTypeId = booking.SlotTypeId.Value,
                LabRoomId = booking.LabRoomId,
                LecturerId = booking.CreatedBy ?? Guid.Empty,
                StudentCount = booking.StudentCount,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                CreatedAt = DateTimeOffset.Now.ToOffset(TimeSpan.Zero),
                CreatedBy = notification.userId,
                ScheduleStatus = ScheduleStatus.Active,
                ScheduleType = ScheduleType.Booking
            };

            await _unitOfWork.Repository<Schedule>().AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
