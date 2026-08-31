using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Domain.Entities
{
    public class BusSeat
    {
        public long Id { get; set; }

        public int BusId { get; set; }

        public string SeatNumber { get; set; } = string.Empty;

        public int RowNumber { get; set; }

        public int ColumnNumber { get; set; }

        public bool IsWindow { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
