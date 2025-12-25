using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class LabRoom : BaseEntity, ISoftDeletable
    {
        public Guid BuildingId { get; set; }
        public Guid ManagerUserId { get; set; }

        public string RoomName { get; set; } = null!;
        public string? Location { get; set; }

        public int Capacity { get; set; }

        public bool IsSpecialized { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }

        public string? Description { get; set; }
    }
}
