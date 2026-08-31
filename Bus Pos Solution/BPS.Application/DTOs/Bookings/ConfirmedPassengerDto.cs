using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Bookings
{
    public class ConfirmedPassengerDto
    {
        public long TripSeatId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public string PassengerName { get; set; } = string.Empty;

        public string PassengerPhone { get; set; } = string.Empty;

        public string? PassengerNID { get; set; }

        public decimal Fare { get; set; }
    }
}
