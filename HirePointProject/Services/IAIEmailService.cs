namespace HirePoint.Services
{
    public interface IAIEmailService
    {
        Task<string> GenerateStatusMessageAsync(
            string applicantName,
            string jobTitle,
            string status,
            string? recruiterMessage);
    }
}