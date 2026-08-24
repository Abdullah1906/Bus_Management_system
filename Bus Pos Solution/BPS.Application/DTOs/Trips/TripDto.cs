using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Trips
{
    public class TripDto
    {
        public long Id { get; set; }

        public int PlaceId { get; set; }

        public string PlaceName { get; set; } = string.Empty;

        public DateTime TripDate { get; set; }

        public bool TipStatus { get; set; }

        public decimal TipAmount { get; set; }

        public decimal Price { get; set; }

        public decimal Total { get; set; }
    }
}
