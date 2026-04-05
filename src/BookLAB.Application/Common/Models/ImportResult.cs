namespace BookLAB.Application.Common.Models
{
    public class ImportResult
    {
        public int TotalProcessed { get; set; }
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public bool Success { get; set; }
    }
    public class ImportValidationResult<T, E>
    {
        public List<RowResult<T, E>> Rows { get; set; } = new();
        public int TotalRows { get; set; }
        public int ErrorCount => Rows.Count(r => r.IsCritical);
        public bool CanCommit => !Rows.Any(r => r.IsCritical);
    }

    public class RowResult<T, E>
    {
        public int RowNumber { get; set; }  
        public T Data { get; set; } = default!;
        public E ConvertedEntity { get; set; } = default!;
        public List<RowError> Errors { get; set; } = new();
        public bool IsCritical => Errors.Any(e => e.Severity == ErrorSeverity.Error);
        public bool HasWarning => Errors.Any(e => e.Severity == ErrorSeverity.Warning);
    }

    public class RowError
    {
        public string? FieldName { get; set; }
        public string Message { get; set; } = default!;
        public ErrorSeverity Severity { get; set; }
        public List<int>? ConflictWithRows { get; set; }
    }

    public enum ErrorSeverity
    {
        Warning = 1,
        Error = 2,
    }
}
