using BookLAB.Application.Common.Specifications;
using BookLAB.Domain.Entities;

namespace BookLAB.Application.Features.Schedules.Queries.GetImportBatches
{
    public class ImportBatchFilterSpecification : BaseSpecification<ImportBatch>
    {
        public ImportBatchFilterSpecification(GetImportBatchesQuery query)
    {
        if (!string.IsNullOrWhiteSpace(query.Name))
            AddCriteria(x => x.Name == query.Name);

        if(!string.IsNullOrWhiteSpace(query.SearchTerm))
            AddCriteria(x => x.Name.ToLower().Contains(query.SearchTerm.ToLower()));

        if (!string.IsNullOrWhiteSpace(query.SemesterName))
            AddCriteria(x => x.SemesterName == query.SemesterName);

        if (query.Type.HasValue)
            AddCriteria(x => x.ImportBatchType == query.Type.Value);

        ApplyOrderBy(x => x.CreatedAt);
    }
}
}