using BPS.Application.DTOs.PasswordReset;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IPasswordResetService
    {
        Task<AuthMessageResponse> ForgotPasswordAsync(
            ForgotPasswordRequest request,
            CancellationToken cancellationToken);

        Task<AuthMessageResponse> ResetPasswordAsync(
            ResetPasswordRequest request,
            CancellationToken cancellationToken);
    }
}
