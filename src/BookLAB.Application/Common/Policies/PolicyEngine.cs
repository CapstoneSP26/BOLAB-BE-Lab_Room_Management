using BookLAB.Application.Common.Interfaces.Services;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Policies
{
    public class PolicyEngine : IPolicyEngine
    {
        private readonly IEnumerable<IBookingPolicyHandler> _handlers;

        public PolicyEngine(IEnumerable<IBookingPolicyHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task ValidateAsync(Booking booking, IEnumerable<RoomPolicy> roomPolicies, CancellationToken ct)
        {
            foreach (var policy in roomPolicies.Where(p => p.IsActive))
            {
                var handler = _handlers.FirstOrDefault(h => h.PolicyKey == policy.PolicyKey);
                if (handler != null)
                {
                    // Execute the specific logic for this PolicyKey
                    await handler.ValidateAsync(booking, policy.PolicyValue, ct);
                }
            }
        }
    }
}
