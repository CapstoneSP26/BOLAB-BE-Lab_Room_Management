using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class Booking : BaseEntity, IAuditable, IUserTrackable
    {
        public int LabRoomId { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public BookingStatus BookingStatus { get; set; }
        public BookingType BookingType {  get; set; }
        public int Recur {  get; set; }
        public string Reason {  get; set; } = string.Empty;
        public int PurposeTypeId { get; set; } 
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public virtual LabRoom LabRoom { get; set; }
        public virtual PurposeType PurposeType { get; set; }
        public virtual User CreatedByUser { get; set; }
        public ICollection<BookingGroup> BookingGroups { get; set; } = new List<BookingGroup>();
    }
}
