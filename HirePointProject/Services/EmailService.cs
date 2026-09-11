using HirePoint.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;

namespace HirePoint.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            settings = options.Value;
        }

        public async Task SendPasswordResetEmailAsync(
            string recipientEmail,
            string resetLink)
        {
            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    settings.SenderName,
                    settings.SenderEmail));

            message.To.Add(
                MailboxAddress.Parse(recipientEmail));

            message.Subject = "HirePoint Password Reset";

            string safeResetLink =
                WebUtility.HtmlEncode(resetLink);

            message.Body = new TextPart("html")
            {
                Text =
                    $"""
                    <h2>Reset Your HirePoint Password</h2>

                    <p>
                        We received a request to reset your HirePoint password.
                    </p>

                    <p>
                        Click the button below to create a new password:
                    </p>

                    <p>
                        <a href="{safeResetLink}"
                           style="
                           background-color:#0d6efd;
                           color:white;
                           padding:10px 20px;
                           text-decoration:none;
                           border-radius:5px;">
                            Reset My Password
                        </a>
                    </p>

                    <p>
                        This link will expire in 30 minutes.
                    </p>

                    <p>
                        If you did not request this password reset,
                        you can safely ignore this email.
                    </p>
                    """
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                settings.SmtpServer,
                settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                settings.SenderEmail,
                settings.Password);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}