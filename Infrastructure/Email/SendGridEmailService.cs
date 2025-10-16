using BookstoreManagementSystem.Application.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace BookstoreManagementSystem.Infrastructure.Email
{
    public class SendGridEmailService : IEmailService
    {
        private readonly SendGridClient _client;
        private readonly SendGridOptions _options;

        public SendGridEmailService(SendGridOptions options)
        {
            _options = options;
            _client = new SendGridClient(_options.ApiKey);
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, CancellationToken ct = default)
        {
            try
            {
                var from = new EmailAddress(_options.FromEmail, _options.FromName);
                var to = new EmailAddress(toEmail);
                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent);
                
                var response = await _client.SendEmailAsync(msg, ct);
                
                // SendGrid returns 2xx for success
                return response.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                // Log the exception in production
                return false;
            }
        }
    }
}
