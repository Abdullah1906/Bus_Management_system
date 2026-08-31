using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Infrastructure.Models
{
    public class TripScheduleResult
    {
        public long Id { get; set; }

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

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }
    }
}
