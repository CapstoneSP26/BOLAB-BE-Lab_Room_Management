using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class BookingRequest : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid BookingId { get; set; }
        public Guid RequestedByUserId { get; set; }

        public BookingApprovalStatus ApprovalStatus { get; set; }

        public Guid? ApprovedByUserId { get; set; }
        public string? ApprovalNotes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
