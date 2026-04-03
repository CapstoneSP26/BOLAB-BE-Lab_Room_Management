using System.Linq.Expressions;

namespace BookLAB.Application.Common.Interfaces.Services
{
    public interface IBackgroundJobService
    {
        // Chạy ngay lập tức với Task bất đồng bộ
        string Enqueue<T>(Expression<Func<T, Task>> methodCall);

        // Chạy sau một khoảng thời gian (Delayed)
        string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay);

        // Chạy định kỳ (Recurring - ví dụ: quét rác mỗi đêm)
        void AddOrUpdateRecurring<T>(string jobId, Expression<Func<T, Task>> methodCall, string cronExpression);
    }
}
