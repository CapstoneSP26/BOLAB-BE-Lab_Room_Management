using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class RoomPolicy : BaseEntity
    {
        public Guid LabRoomId { get; set; }
        public string PolicyKey { get; set; } = null!;
        public string PolicyValue { get; set; } = null!;
    }
}
