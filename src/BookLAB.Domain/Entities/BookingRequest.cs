using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class BookingRequest : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid BookingId { get; set; }
        public Guid RequestedByUserId { get; set; }
        public Guid? ResponsedByUserId { get; set; }
        public BookingRequestStatus BookingRequestStatus { get; set; }
        public string? ResponseContext { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }

        public virtual Booking Booking { get; set; }
        public virtual User Requester { get; set; }
        public virtual User Reponser { get; set; }

    }
}
