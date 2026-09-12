using BPS.Domain.Enums;
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

        public BookingStatus BookingStatus { get; set; }

        public PaymentStatus PaymentStatus { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        public string FromPlaceName { get; set; } = string.Empty;
        public string ToPlaceName { get; set; } = string.Empty;
        public DateTime TripDate { get; set; }
    }
}
