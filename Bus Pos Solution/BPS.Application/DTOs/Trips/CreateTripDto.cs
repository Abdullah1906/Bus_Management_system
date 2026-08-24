using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Trips
{
    public class CreateTripDto
    {
        public int PlaceId { get; set; }

        public DateTime TripDate { get; set; }

        public bool TipStatus { get; set; }

        public decimal TipAmount { get; set; }
    }
}
