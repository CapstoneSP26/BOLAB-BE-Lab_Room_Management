using BookLAB.Application.Common.Interfaces.Services;
using Hangfire;
using System.Linq.Expressions;

namespace BookLAB.Infrastructure.Services
{
    public class HangfireJobService : IBackgroundJobService
    {
        public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
            => BackgroundJob.Enqueue(methodCall);

        public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
            => BackgroundJob.Schedule(methodCall, delay);

        public void AddOrUpdateRecurring<T>(string jobId, Expression<Func<T, Task>> methodCall, string cronExpression)
            => RecurringJob.AddOrUpdate(jobId, methodCall, cronExpression);
    }
}
