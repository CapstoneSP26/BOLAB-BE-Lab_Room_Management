using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class BookingUser : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
    }
}
