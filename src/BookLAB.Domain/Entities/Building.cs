using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Building 
    {
        public int Id { get; set; }
        public int CampusId { get; set; }
        public string BuildingName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string BuildingImageUrl { get; set; } = null!;
        public virtual Campus Campus { get; set; }
    }
}
