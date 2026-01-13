using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class Booking : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid LabRoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public BookingType BookingType {  get; set; }
        public string Reason {  get; set; } = string.Empty;
        public Guid PurposeTypeId { get; set; } 
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public virtual LabRoom LabRoom { get; set; } = new LabRoom();
        public virtual PurposeType PurposeType { get; set; } = new PurposeType();
        public ICollection<BookingGroup> BookingGroups { get; set; } = new List<BookingGroup>();
    }
}
