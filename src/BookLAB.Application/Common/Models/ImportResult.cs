namespace BookLAB.Application.Common.Models
{
    public class ImportValidationResult<T>
    {
        public List<RowResult<T>> Rows { get; set; } = new();
        public int TotalRows { get; set; }
        public int ErrorCount => Rows.Count(r => r.IsCritical);
        public bool CanCommit => !Rows.Any(r => r.IsCritical);
    }

    public class RowResult<T>
    {
        public int RowNumber { get; set; }  
        public T Data { get; set; } = default!;
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
