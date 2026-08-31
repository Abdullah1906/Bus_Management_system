using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Bookings
{
    public class LockedSeatDto
    {
        public long TripSeatId { get; set; }

        public long TripId { get; set; }

        public long BusSeatId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public byte Status { get; set; }

        public DateTime LockedUntil { get; set; }
    }
}
