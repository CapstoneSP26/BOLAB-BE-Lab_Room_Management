using BookLAB.Domain.Common;

namespace BookLAB.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string RoleName { get; set; } = null!;
    }
}
