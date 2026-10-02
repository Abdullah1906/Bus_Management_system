using BPS.Application.DTOs.PasswordReset;
using FluentValidation;

namespace BPS.Application.Validators.Auth
{
    public class ResetPasswordValidator: AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidator()
        {
            RuleFor(x => x.Token)
                .NotEmpty()
                .WithMessage("Reset token is required.");

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("Password is required.")

                .MinimumLength(6)
                .WithMessage(
                    "Password must be at least 6 characters.")

                .MaximumLength(20)
                .WithMessage(
                    "Password cannot exceed 20 characters.");
        }
    }
}
