using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Campus
    {
        public int Id { get; set; }
        public string CampusName { get; set; } = null!;
        public string? Address { get; set; }
        public string? CampusImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<Building> Buildings { get; set; } = new List<Building>();

    }
}
