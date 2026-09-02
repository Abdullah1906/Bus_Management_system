using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Routes
{
    public class CreateRouteDto
    {
        public string FromPlace { get; set; } = string.Empty;

        public string ToPlace { get; set; } = string.Empty;

        public decimal? DistanceKm { get; set; }

        public int? EstimatedMinutes { get; set; }
    }
}
