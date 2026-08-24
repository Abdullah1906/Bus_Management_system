using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Places
{
    public class PlaceDto
    {
        public int Id { get; set; }

        public string PlaceName { get; set; } = string.Empty;

        public decimal PricePerTrip { get; set; }

        public bool IsActive { get; set; }
        public DateTime UpdateAt { get; set; }
    }
}
