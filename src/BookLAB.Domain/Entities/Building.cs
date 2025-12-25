using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Building : BaseEntity
    {
        public Guid CampusId { get; set; }
        public string BuildingName { get; set; } = null!;
        public int NumberOfFloors { get; set; }
    }
}
