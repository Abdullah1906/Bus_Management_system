using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Entities
{
    public class Bus
    {
        public int Id { get; set; }

        public string BusName { get; set; } = string.Empty;

        public string BusNumber { get; set; } = string.Empty;

        public int TotalSeats { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public string? UpdatedBy { get; set; }
    }
}
