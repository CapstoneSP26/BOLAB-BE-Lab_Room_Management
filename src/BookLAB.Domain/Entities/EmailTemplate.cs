using BookLAB.Domain.Enums;

namespace BookLAB.Domain.Entities
{
    public class EmailTemplate
    {
        public int Id { get; set; } 
        public string Subject { get; set; } = string.Empty; 
        public string Content { get; set; } = string.Empty;
        public string VariablesJson { get; set; } = "[]";
        public EmailType Type { get; set; }
    }
}
