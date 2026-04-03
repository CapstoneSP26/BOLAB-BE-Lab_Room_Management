using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Jobs.Emails;
using BookLAB.Application.Common.Jobs.Schedules;
using MediatR;

namespace BookLAB.Application.Features.Bookings.Events
{
    public class BookingRejectedEventHandler : INotificationHandler<BookingRejectedEvent>
    {
        private readonly IBackgroundJobService _jobService;

        public BookingRejectedEventHandler(IBackgroundJobService jobService)
        {
            _jobService = jobService;
        }

        public Task Handle(BookingRejectedEvent notification, CancellationToken cancellationToken)
        {
            _jobService.Enqueue<RejectBookingEmailJob>(
                x => x.Execute(notification.BookingId));

            return Task.CompletedTask;
        }
    }
}
