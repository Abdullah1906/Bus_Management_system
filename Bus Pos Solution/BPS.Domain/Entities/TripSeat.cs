using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Entities
{
    public class TripSeat
    {
        public long Id { get; set; }

        public long TripId { get; set; }

        public long BusSeatId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public byte Status { get; set; }

        public long? LockedByCustomerId { get; set; }

        public DateTime? LockedUntil { get; set; }

        public DateTime? BookedAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
