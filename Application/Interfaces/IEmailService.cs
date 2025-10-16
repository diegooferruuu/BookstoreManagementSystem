namespace BookstoreManagementSystem.Application.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, CancellationToken ct = default);
    }
}
