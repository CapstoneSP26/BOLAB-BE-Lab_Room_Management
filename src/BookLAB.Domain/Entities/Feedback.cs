using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Feedback : BaseEntity, IAuditable, IUserTrackable
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }

        public string FeedbackType { get; set; } = null!;
        public string Description { get; set; } = null!;

        public bool IsResolved { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
    }
}
