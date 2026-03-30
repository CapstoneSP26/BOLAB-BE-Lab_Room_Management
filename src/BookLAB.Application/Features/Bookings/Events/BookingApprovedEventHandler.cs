using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Jobs;
using MediatR;

namespace BookLAB.Application.Features.Bookings.Events
{
    public class BookingApprovedEventHandler : INotificationHandler<BookingApprovedEvent>
    {
        private readonly IBackgroundJobService _jobService;

        public BookingApprovedEventHandler(IBackgroundJobService jobService)
        {
            _jobService = jobService;
        }

        public Task Handle(BookingApprovedEvent notification, CancellationToken cancellationToken)
        {
            // 🔥 Critical
            _jobService.Enqueue<CreateScheduleJob>(
                x => x.Execute(notification.BookingId));

            // 🟡 Low
            _jobService.Enqueue<SendEmailJob>(
                x => x.Execute(notification.BookingId));

            return Task.CompletedTask;
        }
    }
}
