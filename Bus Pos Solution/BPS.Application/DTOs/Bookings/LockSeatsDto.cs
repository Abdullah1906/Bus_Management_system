using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Bookings
{
    public class LockSeatsDto
    {
        public long TripId { get; set; }

        public List<long> TripSeatIds { get; set; } = new();
    }
}
