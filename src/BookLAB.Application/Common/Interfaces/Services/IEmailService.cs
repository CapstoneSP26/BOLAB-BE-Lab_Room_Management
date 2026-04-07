namespace BookLAB.Application.Common.Interfaces.Services
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string to, string body, string subject);
    }
}
