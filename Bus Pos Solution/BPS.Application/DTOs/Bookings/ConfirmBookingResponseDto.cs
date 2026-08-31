using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Bookings
{
    public class ConfirmBookingResponseDto
    {
        public long BookingId { get; set; }

        public string PNR { get; set; } = string.Empty;

        public long TripId { get; set; }

        public long CustomerId { get; set; }

        public decimal TotalAmount { get; set; }

        public byte BookingStatus { get; set; }

        public byte PaymentStatus { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public List<ConfirmedPassengerDto> Passengers { get; set; } = new();
    }
}
