using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class LabOwner : BaseEntity
    {
        public Guid UserId { get; set; }
        public int LabRoomId { get; set; }
        public virtual User User { get; set; } = new User();
        public virtual LabRoom LabRoom { get; set; } = new LabRoom();
    }
}
