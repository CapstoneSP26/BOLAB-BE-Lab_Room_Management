namespace BookLAB.Application.Common.Interfaces.Jobs
{
    public interface IApproveBookingEmailJob
    {
        Task Execute(Guid bookingId);
    }
}