using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Semester : BaseEntity
    {
        public string SemesterCode { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsCurrent { get; set; }
    }
}
