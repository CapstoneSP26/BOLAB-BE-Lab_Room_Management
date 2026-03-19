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
                Criteria = Combine(Criteria, b => b.Booking.BookingStatus == query.Status.Value);

            if (query.LabRoomId.HasValue)
                Criteria = Combine(Criteria, b => b.Booking.LabRoomId == query.LabRoomId.Value);

            if (query.FromDate.HasValue)
                Criteria = Combine(Criteria, b => b.Booking.StartTime >= query.FromDate.Value);

            if (query.ToDate.HasValue)
                Criteria = Combine(Criteria, b => b.Booking.StartTime < query.ToDate.Value);
            // Include related data
            AddInclude(b => b.Booking);
        }

        // Helper to combine expressions (AndAlso)
        private Expression<Func<T, bool>> Combine<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T));
            var body = Expression.AndAlso(
                Expression.Invoke(left, param),
                Expression.Invoke(right, param)
            );
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}
