using BookLAB.Application.Common.Models;
using BookLAB.Domain.Enums;
using MediatR;

namespace BookLAB.Application.Features.Schedules.Queries.GetImportBatches
{
    public class GetImportBatchesQuery : IRequest<PagedList<ImportBatchDto>>
    {
        public string? Name { get; set; }
        public string? SemesterName { get; set; }
        public string SearchTerm { get; set; } = "";

        public ImportBatchType? Type { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 0;
    }
}
