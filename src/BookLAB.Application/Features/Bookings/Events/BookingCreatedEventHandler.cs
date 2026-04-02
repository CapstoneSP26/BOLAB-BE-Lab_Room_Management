using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Application.Common.Jobs.Emails;
using MediatR;

namespace BookLAB.Application.Features.Bookings.Events
{

    public class BookingCreatedEventHandler : INotificationHandler<BookingCreatedEvent>
    {
        private readonly IBackgroundJobService _jobService;

        public BookingCreatedEventHandler(IBackgroundJobService jobService)
        {
            _jobService = jobService;
        }

        public Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
        {
            _jobService.Enqueue<BookingSubmittedEmailJob>(
                x => x.Execute(notification.BookingId));

            _jobService.Enqueue<NotifyAdminNewBookingJob>(
                x => x.Execute(notification.BookingId));

            return Task.CompletedTask;
        }
    }
}
