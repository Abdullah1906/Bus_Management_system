using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Entities
{
    public class Trip
    {
        public long Id { get; set; }

        public int BusId { get; set; }

        public int RouteId { get; set; }

        public DateTime TripDate { get; set; }

        public TimeSpan DepartureTime { get; set; }

        public TimeSpan? ArrivalTime { get; set; }

        public decimal Fare { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
