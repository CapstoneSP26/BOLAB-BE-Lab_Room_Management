using BookLAB.Application.Common.Models;
using BookLAB.Domain.Entities;
using BookLAB.Domain.Enums;

namespace BookLAB.Application.Common.Policies
{
    public interface IPolicyHandler
    {
        PolicyType PolicyType { get; }

        Task<PolicyValidationResult> ValidateAsync(BookingRequest request, string value);
    }
}
