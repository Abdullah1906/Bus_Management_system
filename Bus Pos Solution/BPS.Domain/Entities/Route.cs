using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Entities
{
    public class Route
    {
        public int Id { get; set; }

        public string FromPlace { get; set; } = string.Empty;

        public string ToPlace { get; set; } = string.Empty;

        public decimal? DistanceKm { get; set; }

        public int? EstimatedMinutes { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
