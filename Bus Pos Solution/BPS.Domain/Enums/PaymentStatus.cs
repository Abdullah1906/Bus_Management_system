using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Enums
{
    public enum PaymentStatus : byte
    {
        Pending = 1,
        Paid = 2,
        Failed = 3
    }
}
