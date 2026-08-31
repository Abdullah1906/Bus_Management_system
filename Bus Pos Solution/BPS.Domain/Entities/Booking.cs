using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Entities
{
    public class Booking
    {
        public long Id { get; set; }

        public string PNR { get; set; } = string.Empty;

        public long TripId { get; set; }

        public long CustomerId { get; set; }

        public decimal TotalAmount { get; set; }

        public byte BookingStatus { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ConfirmedAt { get; set; }
    }
}
