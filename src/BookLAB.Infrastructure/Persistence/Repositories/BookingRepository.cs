using BookLAB.Application.Common.Interfaces.Persistence;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BookLAB.Infrastructure.Persistence.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly BookLABDbContext _context;

        public BookingRepository(BookLABDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> IsOverlappedAsync(int labRoomId, DateTime start, DateTime end, int overrideNumber)
        {
            var activeBookingCount = await _context.Bookings
                        .CountAsync(b => b.LabRoomId == labRoomId &&
                                         b.BookingStatus != BookingStatus.Cancelled &&
                                         b.BookingStatus != BookingStatus.Rejected &&
                                         start < b.EndTime && b.StartTime < end);

            return activeBookingCount >= overrideNumber;
        }
    }
}
