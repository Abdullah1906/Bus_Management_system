using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.TripSearch
{
    public class TripSearchDto
    {
        public string FromPlace { get; set; } = string.Empty;
        public string ToPlace { get; set; } = string.Empty;
        public DateTime TripDate { get; set; }
    }
}
