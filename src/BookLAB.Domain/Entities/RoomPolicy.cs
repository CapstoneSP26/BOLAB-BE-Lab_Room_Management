using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class RoomPolicy : BaseEntity, IAuditable, IUserTrackable
    {
        public int LabRoomId { get; set; }
        public PolicyType PolicyKey { get; set; }
        public string PolicyValue { get; set; } = null!;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public bool IsActive { get; set; } = true;
        public virtual LabRoom LabRoom { get; set; }
    }
}
