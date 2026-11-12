using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.StaffPayroll
{
    public class PayrollSummaryResponse
    {
        public long TotalStaff { get; set; }

        public long PaidCount { get; set; }

        public long UnpaidCount { get; set; }

        public decimal TotalPayable { get; set; }

        public decimal TotalPaid { get; set; }

        public decimal TotalUnpaid { get; set; }
    }

    public class MonthlyPayrollResponse
    {
        public IEnumerable<PayrollResponse> Items { get; set; } = [];

        public PayrollSummaryResponse Summary { get; set; } = new();
    }


}
