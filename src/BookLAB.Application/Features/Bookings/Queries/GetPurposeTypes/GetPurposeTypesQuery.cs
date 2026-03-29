using BookLAB.Application.Common.Models;
using MediatR;

namespace BookLAB.Application.Features.Bookings.Queries.GetPurposeTypes
{
    public class GetPurposeTypesQuery : IRequest<PagedList<PurposeTypeDto>>
    {
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 0;
    }
}
