using BookLAB.Domain.Enums;

namespace BookLAB.Application.Features.Schedules.Queries.GetImportBatches
{
    public class ImportBatchDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ImportBatchType ImportBatchType { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }

    }
}
