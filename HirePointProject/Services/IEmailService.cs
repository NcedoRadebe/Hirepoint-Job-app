namespace HirePoint.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(
            string recipientEmail,
            string resetLink);
    }
}