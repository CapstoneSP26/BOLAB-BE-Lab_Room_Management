using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class PurposeType : BaseEntity
    {
        public string PurposeName { get; set; } = string.Empty;
    }
}
