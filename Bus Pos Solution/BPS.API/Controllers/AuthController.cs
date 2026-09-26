using BPS.Application.DTOs.Auth;
using BPS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BPS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(
            IAuthService authService)
        {
            _authService = authService;
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
    }
}
