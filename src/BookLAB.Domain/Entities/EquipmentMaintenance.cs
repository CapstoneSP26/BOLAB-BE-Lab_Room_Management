using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class EquipmentMaintenance : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid EquipmentItemId { get; set; }
        public Guid ReportedByUserId { get; set; }

        public string Description { get; set; } = null!;
        public MaintenanceStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
