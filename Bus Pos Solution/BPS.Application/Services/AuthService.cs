using BPS.Application.DTOs.Auth;
using BPS.Application.Interfaces;
using BPS.Domain.Entities;
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


        public async Task<RegisterResponseDto?> RegisterAsync(RegisterRequestDto request)
        {
            // Check username already exists
            var existingUser =
                await _userRepository.GetByUsernameAsync(
                    request.Username);

            if (existingUser is not null)
                return null;

            // Hash password
            var passwordHash =
                _passwordHasher.HashPassword(
                    request.Password);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = passwordHash,
                FullName = request.FullName,

                // User cannot choose role
                Role = "Customer",

                Email = request.Email,
                PhoneNumber = request.PhoneNumber,

                IsActive = true,

                CreatedBy = request.Username
                
            };

            var userId =
                await _userRepository.CreateAsync(user);

            return new RegisterResponseDto
            {
                UserId = userId,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };
        }
    }
}
