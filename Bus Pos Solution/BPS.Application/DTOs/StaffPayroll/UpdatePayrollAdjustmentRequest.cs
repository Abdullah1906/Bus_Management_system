using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.StaffPayroll
{
    public class UpdatePayrollAdjustmentRequest
    {
        public decimal BonusAmount { get; set; }

        public decimal DeductionAmount { get; set; }

        public string? AdjustmentRemarks { get; set; }
    }
}
