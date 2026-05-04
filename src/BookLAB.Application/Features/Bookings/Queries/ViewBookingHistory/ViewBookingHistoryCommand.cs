using BookLAB.Domain.Entities;
using MediatR;

namespace BookLAB.Application.Features.Bookings.Queries.ViewBookingHistory
{
    public class ViewBookingHistoryCommand : IRequest<List<Booking>>
    {
        public Guid userId { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
        public string status { get; set; }
        public DateTimeOffset startDate { get; set; }
        public DateTimeOffset endDate { get; set; }
        public int? labRoomId { get; set; }
    }
}
