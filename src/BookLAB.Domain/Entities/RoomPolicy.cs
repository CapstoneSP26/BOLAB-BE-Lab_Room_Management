using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class RoomPolicy : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid LabRoomId { get; set; }
        public string PolicyKey { get; set; } = null!;
        public string PolicyValue { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual LabRoom LabRoom { get; set; } = new LabRoom();
    }
}
