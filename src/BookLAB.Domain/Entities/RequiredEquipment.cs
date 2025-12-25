using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class RequiredEquipment : BaseEntity
    {
        public Guid BookingId { get; set; }

        public Guid? EquipmentItemId { get; set; }

        public string EquipmentName { get; set; } = null!;

        public int QuantityRequired { get; set; }
    }
}
