using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class SlotType
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!; 
        public string Name { get; set; } = null!;
        public int CampusId { get; set; } 
        public virtual Campus Campus { get; set; }
        public virtual ICollection<SlotFrame> SlotFrames { get; set; } = new List<SlotFrame>();
    }
}
