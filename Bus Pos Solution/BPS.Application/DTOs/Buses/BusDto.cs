using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Buses
{
    public class BusDto
    {
        public int Id { get; set; }

        public string BusName { get; set; } = string.Empty;

        public string BusNumber { get; set; } = string.Empty;

        public int TotalSeats { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
