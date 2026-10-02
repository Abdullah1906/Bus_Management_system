using BPS.Application.Interfaces;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;


namespace BPS.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetEmailAsync(
            string email,
            string resetLink,
            CancellationToken cancellationToken)
        {
            var host =
                _configuration["Email:Host"]
                ?? throw new InvalidOperationException();

            var port =
                int.Parse(
                    _configuration["Email:Port"] ?? "587");

            var username =
                _configuration["Email:Username"]
                ?? throw new InvalidOperationException();

            var password =
                _configuration["Email:Password"]
                ?? throw new InvalidOperationException();

            var from =
                _configuration["Email:From"]
                ?? username;

            var fromName =
                _configuration["Email:FromName"]
                ?? "BPS";

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    fromName,
                    from));

            message.To.Add(
                MailboxAddress.Parse(email));

            message.Subject =
                "BPS - Password Reset";

            message.Body =
                new BodyBuilder
                {
                    HtmlBody = $"""
                <h2>BPS - Password Reset</h2>

                <p>
                    We received a request to reset your password.
                </p>

                <p>
                    Click the button below to create a new password.
                </p>

                <p>
                    <a href="{resetLink}"
                       style="
                       display:inline-block;
                       padding:12px 20px;
                       background:#1976d2;
                       color:white;
                       text-decoration:none;
                       border-radius:5px;">
                        Reset Password
                    </a>
                </p>

                <p>
                    This link will expire in 30 minutes.
                </p>

                <p>
                    If you did not request a password reset,
                    you can safely ignore this email.
                </p>

                <p>
                    BPS - Bus Position Solution
                </p>
                """
                }.ToMessageBody();

            using var smtp =
                new SmtpClient();

            await smtp.ConnectAsync(
                host,
                port,
                SecureSocketOptions.StartTls,
                cancellationToken);

            await smtp.AuthenticateAsync(
                username,
                password,
                cancellationToken);

            await smtp.SendAsync(
                message,
                cancellationToken);

            await smtp.DisconnectAsync(
                true,
                cancellationToken);
        }
    }

}
