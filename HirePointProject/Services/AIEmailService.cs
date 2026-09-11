#pragma warning disable OPENAI001
using HirePoint.Models;
using Microsoft.Extensions.Options;
using OpenAI.Responses;
using System.Runtime;

namespace HirePoint.Services
{
    public class AIEmailService : IAIEmailService
    {
        private readonly AISettings settings;

        public AIEmailService(
            IOptions<AISettings> options)
        {
            settings = options.Value;
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
                    "OpenAI API key is not configured.");
            }

            if (string.IsNullOrWhiteSpace(settings.Model))
            {
                throw new InvalidOperationException(
                    "OpenAI model is not configured.");
            }

            string prompt = $"""
                You are the applicant communication assistant for HirePoint,
                a job application platform.

                Write a professional, friendly and personalized application
                status message for a jobseeker.

                Applicant name: {applicantName}

                Job title: {jobTitle}

                Application status: {status}

                Recruiter message:
                {recruiterMessage ?? "No additional message was provided."}

                Rules:
                - Address the applicant by their first name if possible.
                - Clearly explain the application status.
                - Be professional and respectful.
                - Keep the message concise.
                - Do not make up information.
                - Do not invent interview dates, salaries, benefits,
                  qualifications or company information.
                - Do not make hiring decisions.
                - Do not mention that AI generated the message.
                - Return only the message that should be sent to the applicant.
                """;

            var client =
                new ResponsesClient(settings.ApiKey);

            var response =
                await client.CreateResponseAsync(
                    settings.Model,
                    prompt);

            string result =
                response.Value.GetOutputText();

            if (string.IsNullOrWhiteSpace(result))
            {
                throw new InvalidOperationException(
                    "OpenAI returned an empty response.");
            }

            return result.Trim();
        }
    }
}