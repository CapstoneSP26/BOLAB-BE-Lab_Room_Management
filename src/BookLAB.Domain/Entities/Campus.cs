using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Campus : BaseEntity
    {
        public string CampusName { get; set; } = null!;
        public string? Address { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
