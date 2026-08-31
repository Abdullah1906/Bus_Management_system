using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.BusSeats
{
    public class CreateBusSeatDto
    {
        public string SeatNumber { get; set; } = string.Empty;

        public int RowNumber { get; set; }

        public int ColumnNumber { get; set; }

        public bool IsWindow { get; set; }
    }
}
