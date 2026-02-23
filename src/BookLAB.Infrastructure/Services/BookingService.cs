using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<Booking>> GetBookingHistoryByUserIdAsync(Guid userId, int page, int limit, string status, DateTimeOffset startDate, DateTimeOffset endDate)
        {
            return await _bookingRepository.GetBookingHistoryByUserIdAsync(userId, page, limit, status, startDate, endDate);
        }
    }
}
