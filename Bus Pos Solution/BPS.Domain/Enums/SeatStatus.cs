using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Enums
{
    public enum SeatStatus : byte
    {
        Available = 1,
        Locked = 2,
        Booked = 3
    }
}
