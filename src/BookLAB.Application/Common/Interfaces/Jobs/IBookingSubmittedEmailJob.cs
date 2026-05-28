namespace BookLAB.Application.Common.Interfaces.Jobs
{
    public interface IBookingSubmittedEmailJob
    {
        Task Execute(Guid bookingId);
    }
}
