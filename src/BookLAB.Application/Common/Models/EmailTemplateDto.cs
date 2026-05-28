namespace BookLAB.Application.Common.Models
{
    public class EmailTemplateDto
    {
        public int Id { get; set; }

        public string Subject { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public int Type { get; set; }

        public List<string> Variables { get; set; } = [];
    }
}
