using System.Collections.Concurrent;
using BookLAB.Application.Common.Interfaces.Integration;
using BookLAB.Application.Common.Interfaces.Repositories;
using BookLAB.Domain.Entities;
using BookLAB.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BookLAB.Infrastructure.Services
{
    public class DashboardRealtimeService : IDashboardRealtimeService
    {
        private static readonly TimeSpan DebounceWindow = TimeSpan.FromMilliseconds(700);

        private readonly IHubContext<NotificationsHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DashboardRealtimeService> _logger;

        private readonly object _debounceLock = new();
        private readonly Dictionary<string, CancellationTokenSource> _debounceByGroup = new();

        public DashboardRealtimeService(
            IHubContext<NotificationsHub> hubContext,
            IServiceScopeFactory scopeFactory,
            ILogger<DashboardRealtimeService> logger)
        {
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task PublishOverviewUpdatedForLabRoomAsync(int labRoomId, string reason, CancellationToken cancellationToken = default)
        {
            if (labRoomId <= 0)
            {
                return;
            }

            var managerIds = await GetManagerIdsByLabRoomIdsAsync(new[] { labRoomId }, cancellationToken);
            QueueDashboardBroadcast(managerIds, reason);
        }

        public async Task PublishOverviewUpdatedForBookingAsync(Guid bookingId, string reason, CancellationToken cancellationToken = default)
        {
            if (bookingId == Guid.Empty)
            {
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var labRoomId = await unitOfWork.Repository<Booking>().Entities
                .Where(x => x.Id == bookingId)
                .Select(x => (int?)x.LabRoomId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!labRoomId.HasValue)
            {
                return;
            }

            await PublishOverviewUpdatedForLabRoomAsync(labRoomId.Value, reason, cancellationToken);
        }

        public async Task PublishOverviewUpdatedForReportAsync(Guid reportId, string reason, CancellationToken cancellationToken = default)
        {
            if (reportId == Guid.Empty)
            {
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var labRoomId = await unitOfWork.Repository<Report>().Entities
                .Where(x => x.Id == reportId)
                .Select(x => (int?)x.Schedule.LabRoomId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!labRoomId.HasValue)
            {
                return;
            }

            await PublishOverviewUpdatedForLabRoomAsync(labRoomId.Value, reason, cancellationToken);
        }

        public async Task PublishOverviewUpdatedForScheduleAsync(Guid scheduleId, string reason, CancellationToken cancellationToken = default)
        {
            if (scheduleId == Guid.Empty)
            {
                return;
            }

            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var labRoomId = await unitOfWork.Repository<Schedule>().Entities
                .Where(x => x.Id == scheduleId)
                .Select(x => (int?)x.LabRoomId)
                .FirstOrDefaultAsync(cancellationToken);

            if (!labRoomId.HasValue)
            {
                return;
            }

            await PublishOverviewUpdatedForLabRoomAsync(labRoomId.Value, reason, cancellationToken);
        }

        private void QueueDashboardBroadcast(IEnumerable<Guid> managerIds, string reason)
        {
            var payload = new
            {
                reason,
                generatedAt = DateTimeOffset.UtcNow
            };

            var groups = new HashSet<string>
            {
                NotificationsHub.GetDashboardAdminGroup()
            };

            foreach (var managerId in managerIds)
            {
                groups.Add(NotificationsHub.GetDashboardLabManagerGroup(managerId.ToString()));
            }

            foreach (var group in groups)
            {
                QueueDebouncedSend(group, payload);
            }
        }

        private void QueueDebouncedSend(string group, object payload)
        {
            CancellationTokenSource cts = new();

            lock (_debounceLock)
            {
                if (_debounceByGroup.TryGetValue(group, out var existingCts))
                {
                    existingCts.Cancel();
                    existingCts.Dispose();
                }

                _debounceByGroup[group] = cts;
            }

            _ = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(DebounceWindow, cts.Token);
                    await _hubContext.Clients.Group(group).SendAsync("dashboard.updated", payload, cts.Token);
                    await _hubContext.Clients.Group(group).SendAsync("dashboard.overview.updated", payload, cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // Debounced by a newer event.
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to broadcast dashboard update event for group {Group}", group);
                }
                finally
                {
                    lock (_debounceLock)
                    {
                        if (_debounceByGroup.TryGetValue(group, out var currentCts) && currentCts == cts)
                        {
                            _debounceByGroup.Remove(group);
                        }
                    }

                    cts.Dispose();
                }
            });
        }

        private async Task<List<Guid>> GetManagerIdsByLabRoomIdsAsync(IEnumerable<int> labRoomIds, CancellationToken cancellationToken)
        {
            var roomIds = labRoomIds.Distinct().ToList();
            if (roomIds.Count == 0)
            {
                return new List<Guid>();
            }

            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            return await unitOfWork.Repository<LabOwner>().Entities
                .Where(x => roomIds.Contains(x.LabRoomId))
                .Select(x => x.UserId)
                .Distinct()
                .ToListAsync(cancellationToken);
        }
    }
}
