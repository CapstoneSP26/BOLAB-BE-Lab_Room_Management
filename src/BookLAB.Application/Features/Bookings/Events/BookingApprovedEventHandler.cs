using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Jobs.Emails;
using BookLAB.Application.Common.Jobs.Schedules;
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
            _jobService.Enqueue<CreateScheduleJob>(
                x => x.Execute(notification.BookingId));

            _jobService.Enqueue<ApproveBookingEmailJob>(
                x => x.Execute(notification.BookingId));

            return Task.CompletedTask;
        }
    }
}
