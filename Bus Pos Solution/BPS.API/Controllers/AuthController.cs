using BPS.Application.DTOs.Auth;
using BPS.Application.DTOs.PasswordReset;
using BPS.Application.Interfaces;
using BPS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IPasswordResetService _passwordResetService;
        public AuthController(
            IAuthService authService,IPasswordResetService passwordResetService)
        {
            _authService = authService;
            _passwordResetService = passwordResetService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(
            LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Password is required.");

            var response =
                await _authService.LoginAsync(request);

            if (response is null)
            {
                return Unauthorized(
                    new
                    {
                        message = "Invalid username or password."
                    });
            }

            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username))
                return BadRequest("Username is required.");

            if (string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Password is required.");

            if (request.Password.Length < 6)
                return BadRequest(
                    "Password must be at least 6 characters.");

            if (string.IsNullOrWhiteSpace(request.FullName))
                return BadRequest("Full name is required.");

            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest("Email is required.");

            var response =
                await _authService.RegisterAsync(request);

            if (response is null)
            {
                return Conflict(
                    new
                    {
                        message = "Username already exists."
                    });
            }

            return StatusCode(
                StatusCodes.Status201Created,
                response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest("Invalid client request.");

            var result = await _authService.RefreshTokenAsync(request);
            if (result is null)
                return Unauthorized(new { message = "Invalid or expired refresh token." });

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request,CancellationToken cancellationToken)
        {
            var result =
                await _passwordResetService
                    .ForgotPasswordAsync(
                        request,
                        cancellationToken);

            return Ok(result);
        }


        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request,CancellationToken cancellationToken)
        {
            var result =
                await _passwordResetService
                    .ResetPasswordAsync(
                        request,
                        cancellationToken);

            return Ok(result);
        }
    }
}
