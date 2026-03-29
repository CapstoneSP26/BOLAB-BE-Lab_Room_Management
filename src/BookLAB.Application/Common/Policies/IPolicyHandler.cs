using BookLAB.Application.Common.Models;
using BookLAB.Application.Features.Bookings.Commands.CreateBooking;
using BookLAB.Domain.Enums;

namespace BookLAB.Application.Common.Policies
{
    public interface IPolicyHandler
    {
        PolicyType PolicyType { get; }

        Task<PolicyValidationResult> ValidateAsync(CreateBookingCommand request, string value);
    }
}
