using Resend;
using BookLAB.Application.Common.Interfaces.Services;

namespace BookLAB.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ResendClient _resend;

        public EmailService(ResendClient resend)
        {
            _resend = resend;
        }

        public async Task SendEmailAsync(
            string to,
            string subject,
            string body)
        {
            var message = new EmailMessage
            {
                From = "BookLAB <noreply@booklab.cloud>",
                Subject = subject,
                HtmlBody = body
            };

            var recipients = to.Split(
                new[] { ',', ';' },
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var recipient in recipients)
            {
                message.To.Add(recipient.Trim());
            }

            var response =
                await _resend.EmailSendAsync(message);

            if (!response.Success)
            {
                throw new Exception(
                    response.Exception.Message);
            }
        }
    }
}