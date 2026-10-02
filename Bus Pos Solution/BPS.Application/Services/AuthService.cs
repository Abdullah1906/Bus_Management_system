using BPS.Application.DTOs.Auth;
using BPS.Application.Interfaces;
using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IJwtTokenService jwtTokenService,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
            _refreshTokenRepository = refreshTokenRepository;
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
                    user,
                    request.Password,
                    user.PasswordHash);

            if (!passwordValid)
                return null;

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshTokenString = _jwtTokenService.GenerateRefreshToken();

            var refreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };
            await _refreshTokenRepository.AddAsync(refreshToken);

            return new LoginResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                UserId = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role
            };
        }


        public async Task<LoginResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var principal = _jwtTokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            if (principal is null) return null;

            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return null;

            var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (storedRefreshToken is null || storedRefreshToken.UserId != userId || storedRefreshToken.IsRevoked || storedRefreshToken.ExpiresAt < DateTime.UtcNow)
                return null;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null || !user.IsActive) return null;

            
            storedRefreshToken.IsRevoked = true;
            await _refreshTokenRepository.UpdateAsync(storedRefreshToken);

            var newAccessToken = _jwtTokenService.GenerateAccessToken(user);
            var newRefreshTokenString = _jwtTokenService.GenerateRefreshToken();

            await _refreshTokenRepository.AddAsync(new RefreshToken
            {
                UserId = user.Id,
                Token = newRefreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            });

            return new LoginResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenString,
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

            var tempUser = new User();
            var passwordHash =
                _passwordHasher.HashPassword(
                    tempUser,
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
