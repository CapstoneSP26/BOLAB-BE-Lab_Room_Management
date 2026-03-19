using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Interfaces.Services
{
    public interface IPolicyEngine
    {
        Task ValidateAsync(Booking booking, IEnumerable<RoomPolicy> roomPolicies, CancellationToken ct);
    }
}
