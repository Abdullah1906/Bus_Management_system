using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Entities
{
    public class TripRecord
    {
        public long Id { get; set; }

        public int PlaceId { get; set; }

        public DateTime TripDate { get; set; }

        public bool TipStatus { get; set; }

        public decimal TipAmount { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
