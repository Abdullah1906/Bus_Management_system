using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.StaffPayroll
{
    public class CreateStaffValidator : AbstractValidator<CreateStaffRequest>
    {
        public CreateStaffValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .MaximumLength(150);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.NIDNumber)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.RoleType)
                .Must(x => new[]
                {
                "Driver",
                "Supervisor",
                "Helper",
                "Office Staff"
                }.Contains(x))
                .WithMessage("Invalid staff role.");

            RuleFor(x => x.JoiningDate)
                .NotEmpty();

            RuleFor(x => x.BaseSalary)
                .GreaterThanOrEqualTo(0);

            // Driver-specific validation
            When(x => x.RoleType == "Driver", () =>
            {
                RuleFor(x => x.DrivingLicenseNumber)
                    .NotEmpty()
                    .MaximumLength(50);

                RuleFor(x => x.LicenseExpiryDate)
                    .NotNull();

                RuleFor(x => x.LicenseType)
                    .Must(x => x == "Heavy" || x == "Medium")
                    .WithMessage("License must be Heavy or Medium.");
            });

            // Non-driver cannot have driver license fields
            When(x => x.RoleType != "Driver", () =>
            {
                RuleFor(x => x.DrivingLicenseNumber)
                    .Must(string.IsNullOrWhiteSpace);

                RuleFor(x => x.LicenseExpiryDate)
                    .Null();

                RuleFor(x => x.LicenseType)
                    .Null();
            });
        }
    }
}
