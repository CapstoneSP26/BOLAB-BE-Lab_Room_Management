using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Interfaces.Repositories
{
    public interface IBookingRepository
    {
        public Task<List<Booking>> GetBookingHistoryByUserIdAsync(Guid userId);
        public Task<bool> IsOverlappedAsync(int labRoomId, DateTime start, DateTime end, int overrideNumber);
    }
}
