using BookLAB.Application.Common.Models;
using MediatR;

namespace BookLAB.Application.Features.IncidentReports.Queries.GetReports
{
    public class GetReportsQuery : IRequest<PagedList<ReportDto>>
    {
        public string? Q { get; set; }
        public int? BuildingId { get; set; }
        public int? RoomId { get; set; }
        public string? ReportType { get; set; }
        public bool? IsResolved { get; set; }
        public DateTimeOffset? FromDate { get; set; }
        public DateTimeOffset? ToDate { get; set; }
        public int? Page { get; set; } = 1;
        public int? Limit { get; set; } = 1000;
        public string? SortBy { get; set; }
        public bool? IsDescending { get; set; }
        public Guid UserId { get; set; }
    }
}