namespace BookLAB.Application.Common.Interfaces.Integration
{
    public interface INotificationService
    {
        Task NotifyNotificationCreatedAsync(Guid userId, object payload, CancellationToken cancellationToken = default);
    }
}
