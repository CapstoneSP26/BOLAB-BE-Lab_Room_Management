using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class LabRoom : ISoftDeletable, IAuditable, IUserTrackable
    {
        public int Id { get; set; } 
        public int BuildingId { get; set; }
        public string RoomName { get; set; } = null!;
        public string? Location { get; set; }
        public int OverrideNumber { get; set; } = 0;
        public bool HasEquipment { get; set; }

        public string? Description { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; }
        
        public virtual Building Building { get; set; }
        public ICollection<LabOwner> LabOwners = new List<LabOwner>();
        public ICollection<RoomPolicy> RoomPolicies = new List<RoomPolicy>();
        public ICollection<LabImage> LabImages = new List<LabImage>();
    }
}
