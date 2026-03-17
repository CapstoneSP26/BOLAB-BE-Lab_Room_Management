using BookLAB.Application.Common.Exceptions;
using BookLAB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookLAB.Application.Common.Policies.Handlers
{
    public class LeadTimePolicyHandler : IBookingPolicyHandler
    {
        public string PolicyKey => "MinLeadTimeHours";

        public Task ValidateAsync(Booking booking, string policyValue, CancellationToken ct)
        {
            if (int.TryParse(policyValue, out int minHours))
            {
                // Logic: StartTime must be at least X hours from now
                if (booking.StartTime < DateTime.UtcNow.AddHours(minHours))
                {
                    throw new BusinessException($"Policy Violation: This room requires at least {minHours} hours lead time.");
                }
            }
            return Task.CompletedTask;
        }
    }
}
