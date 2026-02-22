namespace BookLAB.Application.Common.Interfaces.Persistence
{
    public interface IBookingRepository
    {
        public Task<bool> IsOverlappedAsync(int labRoomId, DateTime start, DateTime end, int overrideNumber);

    }
}
