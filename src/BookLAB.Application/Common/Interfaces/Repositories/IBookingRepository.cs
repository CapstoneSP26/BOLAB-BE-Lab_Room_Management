using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        public Task<bool> IsOverlappedAsync(int labRoomId, DateTime start, DateTime end, int overrideNumber);
        public Task<List<Booking>> GetBookingHistoryByUserIdAsync(Guid userId, int page, int limit, string status, DateTimeOffset startDate, DateTimeOffset endDate);
    }
}
