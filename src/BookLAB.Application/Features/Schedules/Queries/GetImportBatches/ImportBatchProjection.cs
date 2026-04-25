using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Schedules.Queries.GetImportBatches
{
    public static class ImportBatchProjection
    {
        public static IQueryable<ImportBatchDto> SelectImportBatch(
         this IQueryable<ImportBatch> query)
        {
            return query.Select(x => new ImportBatchDto
            {
                Id = x.Id,
                Name = x.Name,
                ImportBatchType = x.ImportBatchType,
                SemesterName = x.SemesterName,
                CreatedAt = x.CreatedAt
            });
        }
    }
}
