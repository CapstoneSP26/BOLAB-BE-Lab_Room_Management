using BookLAB.Domain.Enums;
using MediatR;
using BookLAB.Application.Common.Models;

namespace BookLAB.Application.Features.Bookings.Queries.GetBookings
{
    public class GetBookingsQuery : IRequest<PagedList<BookingDto>>
    {
        // Filter properties
        public string? SearchTerm { get; set; }
        public BookingStatus? Status { get; set; }
        public int? LabRoomId { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public Guid? RequestedBy { get; set; }

        // Pagination & Sorting
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SortBy { get; set; } // e.g., "StartTime", "CreatedAt"
        public bool IsDescending { get; set; }
    }
}
