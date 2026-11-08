using HirePoint.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace HirePoint.Services
{
    public class AIEmailService : IAIEmailService
    {
        private readonly AISettings settings;
        private readonly HttpClient httpClient;

        public AIEmailService(
            IOptions<AISettings> options,
            IHttpClientFactory httpClientFactory)
        {
            settings = options.Value;
            httpClient = httpClientFactory.CreateClient();
        }

        public async Task<string> GenerateStatusMessageAsync(
            string applicantName,
            string jobTitle,
            string status,
            string? recruiterMessage)
        {
            if (string.IsNullOrWhiteSpace(settings.ApiKey))
            {
                throw new InvalidOperationException(
                    "Groq API key is not configured.");
            }

            if (string.IsNullOrWhiteSpace(settings.Model))
            {
                throw new InvalidOperationException(
                    "Groq model is not configured.");
            }

            string prompt = $"""
                You are the applicant communication assistant for HirePoint,
                a professional job application platform.

                Write a professional, friendly and personalized message to
                a jobseeker about their job application.

                Applicant name:
                {applicantName}

                Job title:
                {jobTitle}

                Application status:
                {status}

                Additional applicant and recruiter information:
                {recruiterMessage ?? "No additional information was provided."}

                Instructions:

                - Address the applicant by their first name if possible.
                - Clearly explain the current application status.
                - Use the applicant information only when it is relevant.
                - Incorporate the recruiter's reason or explanation naturally
                  into the message.
                - Be professional, respectful and empathetic.
                - Keep the message concise and suitable for email.
                - Keep the message under 900 characters where possible.
                - Do not make up information.
                - Do not invent interview dates, interview times, salaries,
                  benefits, qualifications, company information or next steps.
                - Do not make hiring decisions.
                - Do not mention that AI generated the message.
                - Do not include a subject line.
                - Return only the message that should be sent to the applicant.
                """;

            var requestBody = new
            {
                model = settings.Model,
                messages = new[]
                {
                    new
                    {
                        role = "system",
                        content =
                            "You write professional job application messages for HirePoint."
                    },
                    new
                    {
                        role = "user",
                        content = prompt
                    }
                },
                temperature = 0.7,
                max_completion_tokens = 500
            };

            string json =
                JsonSerializer.Serialize(requestBody);

            using var request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://api.groq.com/openai/v1/chat/completions");

            request.Headers.Authorization =
                new AuthenticationHeaderValue(
                    "Bearer",
                    settings.ApiKey);

            request.Content =
                new StringContent(
                    json,
                    Encoding.UTF8,
                    "application/json");

            using HttpResponseMessage response =
                await httpClient.SendAsync(request);

            string responseContent =
                await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"Groq API request failed with HTTP {(int)response.StatusCode}: {responseContent}");
            }

            using JsonDocument document =
                JsonDocument.Parse(responseContent);

            string? result =
                document.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString();

            if (string.IsNullOrWhiteSpace(result))
            {
                throw new InvalidOperationException(
                    "Groq returned an empty response.");
            }

            return result.Trim();
        }
    }
}