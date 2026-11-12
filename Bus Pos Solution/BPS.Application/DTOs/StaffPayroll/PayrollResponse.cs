using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.StaffPayroll
{
    public class PayrollResponse
    {
        public long PaymentId { get; set; }

        public int StaffId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string RoleType { get; set; } = string.Empty;

        public DateOnly PayrollMonth { get; set; }

        public decimal BaseSalary { get; set; }

        public decimal BonusAmount { get; set; }

        public decimal DeductionAmount { get; set; }

        public decimal NetPayable { get; set; }

        public string? AdjustmentRemarks { get; set; }

        public string PaymentStatus { get; set; } = string.Empty;

        public string? PaymentMethod { get; set; }

        public string? TransactionRef { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
