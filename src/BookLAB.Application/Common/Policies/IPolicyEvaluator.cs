using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Common.Policies
{
    public interface IPolicyEvaluator
    {
        Task EvaluateAsync(CreateBookingCommand request, IEnumerable<RoomPolicy> policies);
    }
}
