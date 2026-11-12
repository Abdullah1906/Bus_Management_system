using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.StaffPayroll
{
    public class ProcessSalaryPaymentRequest
    {
        public string PaymentMethod { get; set; } = string.Empty;

        public string? TransactionRef { get; set; }

        public string? Remarks { get; set; }
    }
}
