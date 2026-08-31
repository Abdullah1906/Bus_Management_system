using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Bookings
{
    public class PassengerDto
    {
        public long TripSeatId { get; set; }

        public string PassengerName { get; set; } = string.Empty;

        public string PassengerPhone { get; set; } = string.Empty;

        public string? PassengerNID { get; set; }
    }
}
