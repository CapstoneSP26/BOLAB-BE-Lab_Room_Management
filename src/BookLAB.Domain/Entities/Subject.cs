using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Subject : BaseEntity
    {
        public string SubjectCode { get; set; } = string.Empty; // PRN211, SWP391
        public string SubjectName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
