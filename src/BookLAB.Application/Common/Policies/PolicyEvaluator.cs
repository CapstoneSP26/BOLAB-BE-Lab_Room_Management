using BookLAB.Application.Common.Exceptions;
using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Policies
{
    public class PolicyEvaluator : IPolicyEvaluator
    {
        private readonly IEnumerable<IPolicyHandler> _handlers;
        public PolicyEvaluator(IEnumerable<IPolicyHandler> handlers) => _handlers = handlers;

        public async Task EvaluateAsync(CreateBookingCommand request, IEnumerable<RoomPolicy> policies)
        {
            foreach (var handler in _handlers)
            {
                var policy = policies.FirstOrDefault(p => p.PolicyKey == handler.PolicyType && p.IsActive == true);

                if (policy == null)
                    continue;

                    // Nếu có handler cho một policy nhưng policy đó không được kích hoạt, vẫn cần gọi handler để nó có thể áp dụng logic mặc định (nếu có)
                var result = await handler.ValidateAsync(request, policy?.PolicyValue ?? "");
                if (!result.IsSuccess)
                {
                    throw new BusinessException(result.Message ?? "Vi phạm chính sách đặt phòng.");
                }
                
            }
        }
    }
}
