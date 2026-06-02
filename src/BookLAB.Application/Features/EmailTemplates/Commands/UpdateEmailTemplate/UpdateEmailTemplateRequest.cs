namespace BookLAB.Application.Features.EmailTemplates.Commands.UpdateEmailTemplate
{
    public class UpdateEmailTemplateRequest
    {
        public string Subject { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
    }
}