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


        // =========================================================
        // PASSWORD RESET EMAIL
        // =========================================================

        public async Task SendPasswordResetEmailAsync(
            string recipientEmail,
            string resetLink)
        {
            var message =
                new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    settings.SenderName,
                    settings.SenderEmail));

            message.To.Add(
                MailboxAddress.Parse(recipientEmail));

            message.Subject =
                "HirePoint Password Reset";

            string safeResetLink =
                WebUtility.HtmlEncode(resetLink);

            message.Body =
                new TextPart("html")
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

                        <p style="margin-top:25px;">
                            This secure link will expire in 3 days
                            and can only be used once.
                        </p>

                        <p>
                            If you did not request this password reset,
                            you can safely ignore this email.
                        </p>
                        """
                };

            using var smtp =
                new SmtpClient();

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


        // =========================================================
        // APPLICATION STATUS EMAIL
        // =========================================================

        public async Task SendApplicationStatusEmailAsync(
            string recipientEmail,
            string applicantName,
            string jobTitle,
            string status,
            string message)
        {
            var email =
                new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    settings.SenderName,
                    settings.SenderEmail));

            email.To.Add(
                new MailboxAddress(
                    applicantName,
                    recipientEmail));

            email.Subject =
                $"HirePoint Application Update - {jobTitle}";

            email.Body =
                new TextPart("plain")
                {
                    Text = message
                };

            using var smtp =
                new SmtpClient();

            await smtp.ConnectAsync(
                settings.SmtpServer,
                settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                settings.SenderEmail,
                settings.Password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }


        // =========================================================
        // REPRESENTATIVE VERIFICATION EMAIL
        // =========================================================

        public async Task SendRepresentativeVerificationEmailAsync(
            string representativeName,
            string representativeEmail,
            string recruiterName,
            string companyName,
            string knowPersonLink,
            string doNotKnowPersonLink)
        {
            var email =
                new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    settings.SenderName,
                    settings.SenderEmail));

            if (string.IsNullOrWhiteSpace(representativeEmail))
            {
                throw new InvalidOperationException(
                    "The company representative email address is empty.");
            }

            if (!MailboxAddress.TryParse(
                    representativeEmail.Trim(),
                    out var recipient))
            {
                throw new InvalidOperationException(
                    $"The company representative email address is invalid: {representativeEmail}");
            }

            email.To.Add(
                new MailboxAddress(
                    representativeName,
                    recipient.Address));
            email.Subject =
                "HirePoint - Recruiter Verification Request";

            string safeRepresentativeName =
                WebUtility.HtmlEncode(representativeName);

            string safeRecruiterName =
                WebUtility.HtmlEncode(recruiterName);

            string safeCompanyName =
                WebUtility.HtmlEncode(companyName);

            string safeKnowPersonLink =
                WebUtility.HtmlEncode(knowPersonLink);

            string safeDoNotKnowPersonLink =
                WebUtility.HtmlEncode(doNotKnowPersonLink);

            email.Body =
                new TextPart("html")
                {
                    Text =
                        $"""
                        <h2>HirePoint Recruiter Verification</h2>

                        <p>
                            Dear {safeRepresentativeName},
                        </p>

                        <p>
                            We are contacting you because
                            <strong>{safeRecruiterName}</strong>
                            has registered on HirePoint and has indicated
                            that they are associated with
                            <strong>{safeCompanyName}</strong>.
                        </p>

                        <p>
                            You were provided as the company representative
                            during this recruiter's registration.
                        </p>

                        <p>
                            Please confirm whether you know this person.
                        </p>

                        <p>
                            Your response will be sent to the HirePoint
                            administrator handling the recruiter's
                            registration.
                        </p>

                        <p style="margin-top:30px;">
                            <a href="{safeKnowPersonLink}"
                               style="
                               background-color:#198754;
                               color:white;
                               padding:12px 22px;
                               text-decoration:none;
                               border-radius:6px;
                               display:inline-block;">
                                I know this person
                            </a>
                        </p>

                        <p style="margin-top:15px;">
                            <a href="{safeDoNotKnowPersonLink}"
                               style="
                               background-color:#dc3545;
                               color:white;
                               padding:12px 22px;
                               text-decoration:none;
                               border-radius:6px;
                               display:inline-block;">
                                I don't know this person
                            </a>
                        </p>

                        <p style="margin-top:30px;">
                            If you did not expect this request, you can
                            safely ignore this email.
                        </p>

                        <p>
                            Thank you,<br />
                            <strong>HirePoint</strong>
                        </p>
                        """
                };

            using var smtp =
                new SmtpClient();
            try
            {
                await smtp.ConnectAsync(
                    settings.SmtpServer,
                    settings.Port,
                    SecureSocketOptions.StartTls);

                await smtp.AuthenticateAsync(
                    settings.SenderEmail,
                    settings.Password);

                await smtp.SendAsync(email);
            }
            finally
            {
                if (smtp.IsConnected)
                {
                    await smtp.DisconnectAsync(true);
                }
            }
        }


        // =========================================================
        // RECRUITER APPROVAL EMAIL
        // =========================================================

        public async Task SendRecruiterApprovalEmailAsync(
            string recruiterName,
            string recruiterEmail,
            string companyName,
            string createPasswordLink)
        {
            var email =
                new MimeMessage();

            email.From.Add(
                new MailboxAddress(
                    settings.SenderName,
                    settings.SenderEmail));

            email.To.Add(
                new MailboxAddress(
                    recruiterName,
                    recruiterEmail));

            email.Subject =
                "HirePoint - Recruiter Account Approved";

            string safeRecruiterName =
                WebUtility.HtmlEncode(recruiterName);

            string safeCompanyName =
                WebUtility.HtmlEncode(companyName);

            string safeCreatePasswordLink =
                WebUtility.HtmlEncode(createPasswordLink);

            email.Body =
                new TextPart("html")
                {
                    Text =
                        $"""
                        <h2>HirePoint Recruiter Account Approved</h2>

                        <p>
                            Dear {safeRecruiterName},
                        </p>

                        <p>
                            Your recruiter registration has been approved
                            by the administrator of
                            <strong>{safeCompanyName}</strong>.
                        </p>

                        <p>
                            Your recruiter account is now ready to be activated.
                        </p>

                        <p>
                            Please click the button below to create your
                            HirePoint password:
                        </p>

                        <p style="margin-top:25px;">
                            <a href="{safeCreatePasswordLink}"
                               style="
                               background-color:#198754;
                               color:white;
                               padding:12px 22px;
                               text-decoration:none;
                               border-radius:6px;
                               display:inline-block;">
                                Create My Password
                            </a>
                        </p>

                        <p style="margin-top:25px;">
                            This secure link will expire in 3 days
                            and can only be used once.
                        </p>

                        <p>
                            After creating your password, you can log in
                            to your HirePoint recruiter account.
                        </p>

                        <p style="margin-top:30px;">
                            If you did not expect this email, please contact
                            the administrator of your company.
                        </p>

                        <p>
                            Thank you,<br />
                            <strong>HirePoint</strong>
                        </p>
                        """
                };

            using var smtp =
                new SmtpClient();

            await smtp.ConnectAsync(
                settings.SmtpServer,
                settings.Port,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                settings.SenderEmail,
                settings.Password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}