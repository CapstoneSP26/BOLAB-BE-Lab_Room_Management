using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class SlotType : BaseEntity
    {
        public string Code { get; set; } = null!; 
        public string Name { get; set; } = null!;
        public Guid CampusId { get; set; } 
        public virtual Campus Campus { get; set; } = new Campus();
        public virtual ICollection<SlotFrame> SlotFrames { get; set; } = new List<SlotFrame>();
    }
}
