using BookLAB.Domain.Enums;
using MediatR;

namespace BookLAB.Application.Features.Bookings.Commands.CreateBooking
{
    public record CreateBookingCommand : IRequest<CreateBookingResponse>
    {
        public int LabRoomId { get; init; }
        public int SlotTypeId { get; init; }
        public int PurposeTypeId { get; init; }
        public DateTimeOffset StartTime { get; init; }
        public DateTimeOffset EndTime { get; init; }
        public int StudentCount { get; init; }
        public int RecurringCount { get; init; }
        public BookingType BookingType { get; init; }
        public string Reason { get; init; } = string.Empty;
        public List<Guid>? GroupIds { get; init; } 
    }
}
