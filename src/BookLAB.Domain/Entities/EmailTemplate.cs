using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class EmailTemplate
    {
        public int Id { get; set; } 
        public string Content { get; set; } = string.Empty;
        public EmailType Type { get; set; }
    }
}
