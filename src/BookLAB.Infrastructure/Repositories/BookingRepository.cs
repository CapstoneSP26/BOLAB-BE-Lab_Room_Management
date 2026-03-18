using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;
using BookLAB.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly BookLABDbContext _context;

        public BookingRepository(BookLABDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetBookingHistoryByUserIdAsync(Guid userId)
        {
            return await _context.Bookings.Where(b => b.CreatedBy == userId).ToListAsync();
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

        public async Task<List<Booking>> GetBookingHistoryByUserIdAsync(Guid userId, int page, int limit, string status, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _context.Bookings.Include(x => x.LabRoom).Include(x => x.LabRoom.Building).Include(x => x.PurposeType)
                .Where(b => b.CreatedBy == userId && (b.BookingStatus.ToString() == status || status.Equals("all")) && b.StartTime >= startDate && b.EndTime <= endDate)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }
    }
}
