using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Entities
{
    public class BookingDetail
    {
        public long Id { get; set; }

        public long BookingId { get; set; }

        public long TripSeatId { get; set; }

        public string PassengerName { get; set; } = string.Empty;

        public string PassengerPhone { get; set; } = string.Empty;

        public string? PassengerNID { get; set; }

        public decimal Fare { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
