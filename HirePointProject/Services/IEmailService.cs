namespace HirePoint.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(
            string recipientEmail,
            string resetLink);

        Task SendApplicationStatusEmailAsync(
            string recipientEmail,
            string applicantName,
            string jobTitle,
            string status,
            string message);

        Task SendRepresentativeVerificationEmailAsync(
            string representativeName,
            string representativeEmail,
            string recruiterName,
            string companyName,
            string knowPersonLink,
            string doNotKnowPersonLink);

        Task SendRecruiterApprovalEmailAsync(
            string recruiterName,
            string recruiterEmail,
            string companyName,
            string createPasswordLink);
    }
}