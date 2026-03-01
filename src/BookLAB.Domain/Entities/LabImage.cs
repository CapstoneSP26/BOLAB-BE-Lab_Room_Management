using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class LabImage : BaseEntity
    {
        public int LabRoomId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Size { get; set; }
        public FileType FileType { get; set; }
        public bool IsAvatar { get; set; }
        public virtual LabRoom LabRoom { get; set; }
    }
}
