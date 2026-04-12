using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookLAB.Infrastructure.BackgroundJobs
{
    public class JobHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public JobHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var scheduler = scope.ServiceProvider.GetRequiredService<RecurringJobScheduler>();
            scheduler.Schedule();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}