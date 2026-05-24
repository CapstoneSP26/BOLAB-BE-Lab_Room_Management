namespace BookLAB.Application.Common.Interfaces.Jobs
{
    public interface IRejectBookingEmailJob
    {
        Task Execute(Guid bookingId);
    }
}
