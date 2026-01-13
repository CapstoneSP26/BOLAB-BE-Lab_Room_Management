using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class SlotFrame : BaseEntity
    {
        public Guid SlotTypeId { get; set; }
        public TimeOnly StartTimeSlot { get; set; }
        public TimeOnly EndTimeSlot { get; set; }
        public int OrderIndex { get; set; }
        public SlotType SlotType { get; set; } = new SlotType();
    }
}
