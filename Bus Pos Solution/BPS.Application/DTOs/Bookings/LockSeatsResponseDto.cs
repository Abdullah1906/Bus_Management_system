using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Bookings
{
    public class LockSeatsResponseDto
    {
        public long TripId { get; set; }

        public DateTime LockedUntil { get; set; }

        public List<LockedSeatDto> Seats { get; set; } = new();
    }
}
