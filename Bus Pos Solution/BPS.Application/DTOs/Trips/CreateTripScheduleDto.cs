using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Trips
{
    public class CreateTripScheduleDto
    {
        public int BusId { get; set; }

        public int RouteId { get; set; }

        public DateTime TripDate { get; set; }

        public TimeSpan DepartureTime { get; set; }

        public TimeSpan? ArrivalTime { get; set; }

        public decimal Fare { get; set; }
    }
}
