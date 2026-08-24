using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Places
{
    public class CreatePlaceDto
    {
        public string PlaceName { get; set; } = string.Empty;

        public decimal PricePerTrip { get; set; }
    }
}
