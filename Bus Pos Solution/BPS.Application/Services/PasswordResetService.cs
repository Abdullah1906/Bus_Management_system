using BPS.Application.DTOs.PasswordReset;
using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly IPasswordResetRepository _repository;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher<User> _passwordHasher;

        private readonly IConfiguration _configuration;

        public PasswordResetService(
            IPasswordResetRepository repository,
            ITokenGenerator tokenGenerator,
            IEmailService emailService,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration)
        {
            _repository = repository;
            _tokenGenerator = tokenGenerator;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public async Task<AuthMessageResponse> ForgotPasswordAsync(
            ForgotPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var email = request.Email.Trim();

            var userId =
                await _repository.GetUserIdByEmailAsync(
                    email,
                    cancellationToken);

            // Do not reveal whether email exists
            if (userId == null)
            {
                return new AuthMessageResponse
                {
                    Message =
                        "If an account exists for this email, " +
                        "a password reset link has been sent."
                };
            }

            var rawToken =
                _tokenGenerator.GeneratePasswordResetToken();

            var tokenHash =
                _tokenGenerator.HashToken(rawToken);

            var resetToken = new PasswordResetToken
            {
                UserId = userId.Value,
                TokenHash = tokenHash,
                ExpiresAt =
                    DateTime.UtcNow.AddMinutes(30),
                CreatedAt = DateTime.UtcNow
            };

            await _repository.CreateTokenAsync(
                resetToken,
                cancellationToken);

            var frontendUrl =
                _configuration["Frontend:BaseUrl"]
                ?? throw new InvalidOperationException(
                    "Frontend URL is not configured.");

            var resetLink =
                $"{frontendUrl}/auth/reset-password" +
                $"?token={Uri.EscapeDataString(rawToken)}";

            await _emailService.SendPasswordResetEmailAsync(
                email,
                resetLink,
                cancellationToken);

            return new AuthMessageResponse
            {
                Message =
                    "If an account exists for this email, " +
                    "a password reset link has been sent."
            };
        }

        public async Task<AuthMessageResponse> ResetPasswordAsync(
            ResetPasswordRequest request,
            CancellationToken cancellationToken)
        {
            var token =
                request.Token.Trim();

            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ArgumentException(
                    "Invalid reset token.");
            }

            var tokenHash =
                _tokenGenerator.HashToken(token);

            // Create a lightweight user object
            // only for password hashing.
            var passwordHash =
                _passwordHasher.HashPassword(
                    new User(),
                    request.NewPassword);

            var userId =
                await _repository.ResetPasswordAsync(
                    tokenHash,
                    passwordHash,
                    cancellationToken);

            if (userId == null)
            {
                throw new ArgumentException(
                    "Invalid or expired reset token.");
            }

            return new AuthMessageResponse
            {
                Message =
                    "Your password has been reset successfully."
            };
        }
    }
}
