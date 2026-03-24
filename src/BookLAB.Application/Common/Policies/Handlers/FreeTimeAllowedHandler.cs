using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;

namespace BookLAB.Application.Common.Policies.Handlers
{
    public class FreeTimeAllowedHandler : IPolicyHandler
    {
        public PolicyType PolicyType => PolicyType.IsFreeTimeAllowed;

        public Task<PolicyValidationResult> ValidateAsync(BookingRequest request, string value)
        {
            throw new NotImplementedException();
        }
    }
}
