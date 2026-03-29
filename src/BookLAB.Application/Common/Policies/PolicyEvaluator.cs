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
            foreach (var policy in policies.Where(p => p.IsActive))
            {
                var handler = _handlers.FirstOrDefault(h => h.PolicyType == policy.PolicyKey);
                if (handler != null)
                {
                    var result = await handler.ValidateAsync(request, policy.PolicyValue);
                    if (!result.IsSuccess)
                    {
                        // Ném BusinessException ở đây để tầng Handler của MediatR bắt được
                        throw new BusinessException(result.Message ?? "Vi phạm chính sách đặt phòng.");
                    }
                }
            }
        }
    }
}
