using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.TripSearch
{
    public class TripSearchResponseDto
    {
        public long TripId { get; set; }

        public int BusId { get; set; }
        public string BusName { get; set; } = string.Empty;
        public string BusNumber { get; set; } = string.Empty;

        public int RouteId { get; set; }
        public string FromPlace { get; set; } = string.Empty;
        public string ToPlace { get; set; } = string.Empty;

        public DateTime TripDate { get; set; }

        public TimeSpan DepartureTime { get; set; }
        public TimeSpan? ArrivalTime { get; set; }

        public decimal Fare { get; set; }

        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
    }
}
