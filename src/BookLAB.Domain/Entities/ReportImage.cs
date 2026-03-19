using BookLAB.Domain.Common;
using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class ReportImage : BaseEntity
    {
        public Guid ReportId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Size { get; set; }
        public FileType FileType { get; set; }
        public virtual Report Report { get; set; }
    }
}
