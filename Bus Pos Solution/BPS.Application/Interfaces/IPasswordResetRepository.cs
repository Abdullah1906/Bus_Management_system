using BPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IPasswordResetRepository
    {
        Task<int?> GetUserIdByEmailAsync(
            string email,
            CancellationToken cancellationToken);

        Task CreateTokenAsync(
            PasswordResetToken token,
            CancellationToken cancellationToken);

        Task<int?> ResetPasswordAsync(
            string tokenHash,
            string passwordHash,
            CancellationToken cancellationToken);
    }
}
