using BPS.Application.DTOs.PasswordReset;
using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;

        public PasswordResetService(
            IPasswordResetRepository repository,
            ITokenGenerator tokenGenerator,
            IEmailService emailService,
            IPasswordHasher<User> passwordHasher,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            HttpClient httpClient)
        {
            _repository = repository;
            _tokenGenerator = tokenGenerator;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }

        public async Task<AuthMessageResponse> ForgotPasswordAsync(ForgotPasswordRequest request,CancellationToken cancellationToken)
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

            //// ip address and timezone logic
            //var httpContext = _httpContextAccessor.HttpContext;
            //string userIpAddress = httpContext?.Connection.RemoteIpAddress?.ToString();
            //string timeZoneId = await GetTimeZoneFromIpAsync(userIpAddress);

            //TimeZoneInfo targetZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
            //DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, targetZone);

            var resetToken = new PasswordResetToken
            {
                UserId = userId.Value,
                TokenHash = tokenHash,
                ExpiresAt =
                    DateTime.UtcNow.AddMinutes(30),
                CreatedAt = DateTime.UtcNow
                //ExpiresAt = localTime.AddMinutes(30),
                //CreatedAt = localTime
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

        /// <summary>
        /// Gets the timezone based on the user's IP address using the ip-api.com service.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        private async Task<string> GetTimeZoneFromIpAsync(string ipAddress)
        {
            try
            {
                string url = $"http://ip-api.com/json/{ipAddress}?fields=timezone";
                var response = await _httpClient.GetStringAsync(url);

                var json = JObject.Parse(response);
                string timezone = json["timezone"]?.ToString();

                return !string.IsNullOrEmpty(timezone) ? timezone : "America/New_York";
            }
            catch
            {
                return "America/New_York"; 
            }
        }
    }
}
