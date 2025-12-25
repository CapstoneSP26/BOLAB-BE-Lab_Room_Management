using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class SlotType : BaseEntity
    {
        public string Code { get; set; } = null!;
        // VD: "OLD_SLOT", "NEW_SLOT", "OUT_SLOT"

        public string Name { get; set; } = null!;
        // VD: "Old Slot", "New Slot", "Out Slot"

        public int? DurationMinutes { get; set; }
        // null = flexible duration

        public bool IsFixedDuration { get; set; }

        public bool RequiresApproval { get; set; }

        public bool AllowsOverCapacity { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
