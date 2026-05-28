namespace BookLAB.Application.Common.Interfaces.Jobs
{
    public interface IRejectBookingByPriorityEmailJob
    {
        Task Execute(List<Guid> bookingIds, List<Guid> scheduleIds);
    }
}
