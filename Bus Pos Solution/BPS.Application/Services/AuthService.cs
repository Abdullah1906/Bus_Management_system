using BPS.Application.DTOs.Auth;
using BPS.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponseDto?> LoginAsync(
            LoginRequestDto request)
        {
            var user =
                await _userRepository.GetByUsernameAsync(
                    request.Username);

            if (user is null)
                return null;

            if (!user.IsActive)
                return null;

            var passwordValid =
                _passwordHasher.VerifyPassword(
                    request.Password,
                    user.PasswordHash);

            if (!passwordValid)
                return null;

            var token =
                _jwtTokenService.GenerateToken(user);

            return new LoginResponseDto
            {
                Token = token,
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };
        }
    }
}
