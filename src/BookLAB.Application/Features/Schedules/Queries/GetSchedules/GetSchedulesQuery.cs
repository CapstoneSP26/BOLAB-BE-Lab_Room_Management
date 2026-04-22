using BookLAB.Application.Common.Models;
using BookLAB.Domain.Enums;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Queries.GetSchedules
{
    public class GetSchedulesQuery : IRequest<PagedList<ScheduleDto>>
    {
        // Filter properties
        public string? SearchTerm { get; set; }
        public string? SearchItems { get; set; }
        public ScheduleStatus? Status { get; set; }
        public int? BuildingId { get; set; }
        public int? LabRoomId { get; set; }
        public Guid? LecturerId { get; set; }
        public string? SubjectCode { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public ScheduleStatus? ExcludedStatus { get; set; }

        // Pagination & Sorting
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 0;
        public string? SortBy { get; set; } // e.g., "StartTime", "CreatedAt"
        public bool IsDescending { get; set; }

    }
}