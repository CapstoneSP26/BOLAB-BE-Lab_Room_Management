using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class Booking : BaseEntity, IAuditable, IUserTrackable
    {
        public int LabRoomId { get; set; }
        public int SlotTypeId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public BookingType BookingType {  get; set; }
        public int StudentCount { get; set; }
        public int Recur {  get; set; }
        public string Reason {  get; set; } = string.Empty;
        public int PurposeTypeId { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public virtual LabRoom LabRoom { get; set; } = new LabRoom();
        public virtual PurposeType PurposeType { get; set; } = new PurposeType();
        public virtual SlotType SlotType { get; set; } = new SlotType();
        public ICollection<BookingGroup> BookingGroups { get; set; } = new List<BookingGroup>();
    }
}
