using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Bookings
{
    public class ConfirmBookingDto
    {
        public long TripId { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string? TransactionId { get; set; }

        public List<PassengerDto> Passengers { get; set; } = new();
    }
}
