using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;

namespace BookLAB.Infrastructure.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<List<Booking>> GetBookingHistoryByUserIdAsync(Guid userId)
        {
            return await _bookingRepository.GetBookingHistoryByUserIdAsync(userId);
        }
    }
}
