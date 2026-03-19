namespace BookLAB.Application.Common.Models
{
    public class ImportValidationResult<T>
    {
        public List<RowResult<T>> Rows { get; set; } = new();
        public bool CanCommit => !Rows.Any(r => r.IsCritical);
    }

    public class RowResult<T>
    {
        public int RowNumber { get; set; }  
        public T Data { get; set; } = default!;
        public string Status { get; set; } = "Valid"; // Valid, Warning, Invalid
        public List<string> Messages { get; set; } = new();
        public bool IsCritical { get; set; }
    }
}
