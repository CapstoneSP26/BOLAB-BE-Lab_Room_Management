using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class BookingGroup : BaseEntity
    {
        public Guid BookingId { get; set; }
        public Guid GroupId { get; set; }
    }
}
