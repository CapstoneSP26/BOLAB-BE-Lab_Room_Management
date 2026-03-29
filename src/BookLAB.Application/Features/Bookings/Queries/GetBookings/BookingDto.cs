using BookLAB.Domain.Enums;

namespace BookLAB.Application.Features.Bookings.Queries.GetBookings
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public int LabRoomId { get; set; }
        public string LabRoomName { get; set; } = string.Empty;

        public Guid CreatedBy { get; set; }
        public string UserFullName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserCode {  get; set; } = string.Empty;

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public int StudentCount { get; set; }
        public string? Reason { get; set; } 

        public BookingStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }

        public bool IsOverdue { get; set; } 
        public bool CanApprove { get; set; } 
        public bool CanReject { get; set; }
    }

}