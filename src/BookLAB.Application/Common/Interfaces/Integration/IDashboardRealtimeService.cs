namespace BookLAB.Application.Common.Interfaces.Integration
{
    public interface IDashboardRealtimeService
    {
        Task PublishOverviewUpdatedForLabRoomAsync(int labRoomId, string reason, CancellationToken cancellationToken = default);
        Task PublishOverviewUpdatedForBookingAsync(Guid bookingId, string reason, CancellationToken cancellationToken = default);
        Task PublishOverviewUpdatedForReportAsync(Guid reportId, string reason, CancellationToken cancellationToken = default);
        Task PublishOverviewUpdatedForScheduleAsync(Guid scheduleId, string reason, CancellationToken cancellationToken = default);
    }
}