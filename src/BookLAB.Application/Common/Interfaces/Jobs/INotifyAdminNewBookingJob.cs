namespace BookLAB.Application.Common.Interfaces.Jobs
{
    public interface INotifyAdminNewBookingJob
    {
        Task Execute(Guid bookingId);
    }
}
