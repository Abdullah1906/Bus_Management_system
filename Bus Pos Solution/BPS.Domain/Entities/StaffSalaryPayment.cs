using BPS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Entities
{
    public class StaffSalaryPayment
    {
        public long PaymentId { get; set; }

        public int StaffId { get; set; }

        public DateOnly PayrollMonth { get; set; }

        public decimal BaseSalary { get; set; }

        public decimal BonusAmount { get; set; }

        public decimal DeductionAmount { get; set; }

        public decimal NetPayable { get; set; }

        public string? AdjustmentRemarks { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }

        public string? TransactionRef { get; set; }

        public DateTime? PaidAt { get; set; }
    }
}
