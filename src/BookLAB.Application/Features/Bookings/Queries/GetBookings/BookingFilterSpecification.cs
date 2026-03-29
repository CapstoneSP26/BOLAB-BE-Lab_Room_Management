using BookLAB.Application.Common.Specifications;
using BookLAB.Domain.Entities;
using System.Linq.Expressions;

namespace BookLAB.Application.Features.Bookings.Queries.GetBookings
{
    public class BookingFilterSpecification : BaseSpecification<BookingRequest>
    {
        public BookingFilterSpecification(GetBookingsQuery query)
        {
            // Default: Not deleted
            //Criteria = b => !b.IsDeleted;

            // Dynamic Filtering
            if (query.Status.HasValue)
                AddCriteria(b => b.Booking.BookingStatus == query.Status.Value);

            if (query.LabRoomId.HasValue)
                AddCriteria(b => b.Booking.LabRoomId == query.LabRoomId.Value);

            if (query.FromDate.HasValue)
                AddCriteria(b => b.Booking.StartTime >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                AddCriteria(b => b.Booking.StartTime < query.ToDate.Value);

            if (query.Status.HasValue)
            {
                AddCriteria(b => b.Booking.BookingStatus == query.Status.Value);
            }

            ApplyOrderBy(b => b.Booking.StartTime);

        }

    }
}
