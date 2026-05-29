using BookLAB.Application.Common.Models;
using MediatR;

namespace BookLAB.Application.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingCommand : IRequest<ResultMessage<bool>>
    {
        public Guid BookingId { get; set; }
        public string? CancelReason { get; set; } // Nhận lý do hủy từ Frontend (Bắt buộc cho Schedule/BookingRequest đặc thù)
    }
}
