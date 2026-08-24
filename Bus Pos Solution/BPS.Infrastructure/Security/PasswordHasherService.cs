using BPS.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Infrastructure.Security
{
    public class PasswordHasherService : IPasswordHasher
    {
        private readonly PasswordHasher<object> _hasher = new();

        public string HashPassword(string password)
        {
            return _hasher.HashPassword(new object(), password);
        }

        public bool VerifyPassword(
            string password,
            string passwordHash)
        {
            var result = _hasher.VerifyHashedPassword(
                new object(),
                passwordHash,
                password);

            return result == PasswordVerificationResult.Success ||
                   result == PasswordVerificationResult.SuccessRehashNeeded;
        }
    }
}
