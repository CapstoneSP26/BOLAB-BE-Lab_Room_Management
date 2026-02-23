using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookLABDbContext _context;
        public BookingRepository(BookLABDbContext context)
        {
            _context = context;
        }

        public async Task<List<Booking>> GetBookingHistoryByUserIdAsync(Guid userId)
        {
            return await _context.Bookings.Where(b => b.CreatedBy == userId).ToListAsync();
        }

        public async Task<List<Booking>> GetBookingHistoryByUserIdAsync(Guid userId, int page, int limit, string status, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _context.Bookings.Where(b => b.CreatedBy == userId && b.BookingStatus.ToString() == status && b.StartTime >= startDate && b.EndTime <= endDate)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();
        }
    }
}
