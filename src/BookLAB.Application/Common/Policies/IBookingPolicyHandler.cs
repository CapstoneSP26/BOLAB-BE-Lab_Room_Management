using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Policies
{
    public interface IBookingPolicyHandler
    {
        // Matches the PolicyKey in your RoomPolicy entity
        string PolicyKey { get; }

        // Logic to validate the booking against the policy value
        Task ValidateAsync(Booking booking, string policyValue, CancellationToken ct);
    }
}
