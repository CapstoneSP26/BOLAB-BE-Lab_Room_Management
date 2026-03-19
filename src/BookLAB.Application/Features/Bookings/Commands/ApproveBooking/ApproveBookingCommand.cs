using MediatR;

namespace BookLAB.Application.Features.Bookings.Commands.ApproveBooking
{
    public record ApproveBookingCommand(Guid BookingId) : IRequest<bool>;
}
