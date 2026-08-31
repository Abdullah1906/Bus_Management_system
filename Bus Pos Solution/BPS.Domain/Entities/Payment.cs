using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Entities
{
    public class Payment
    {
        public long Id { get; set; }

        public long BookingId { get; set; }

        public decimal Amount { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string? TransactionId { get; set; }

        public byte PaymentStatus { get; set; }

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
