using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Enums
{
    public enum BookingStatus : byte
    {
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3
    }
}
