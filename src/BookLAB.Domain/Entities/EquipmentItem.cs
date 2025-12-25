using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class EquipmentItem : BaseEntity, ISoftDeletable
    {
        public string ItemName { get; set; } = null!;
        public string SerialNumber { get; set; } = null!;

        public Guid? RoomId { get; set; }

        public EquipmentCondition Condition { get; set; }
        public bool IsDeleted { get; set; }
    }
}
