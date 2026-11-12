using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.StaffPayroll
{
    public class ProcessSalaryPaymentValidator: AbstractValidator<ProcessSalaryPaymentRequest>
    {
        public ProcessSalaryPaymentValidator()
        {
            RuleFor(x => x.PaymentMethod)
                .Must(x => new[]
                {
                "Cash",
                "Bkash",
                "Bank Transfer"
                }.Contains(x))
                .WithMessage("Invalid payment method.");

            When(
                x => x.PaymentMethod is "Bkash" or "Bank Transfer",
                () =>
                {
                    RuleFor(x => x.TransactionRef)
                        .NotEmpty()
                        .MaximumLength(150);
                });

            RuleFor(x => x.Remarks)
                .MaximumLength(500);
        }
    }
}
