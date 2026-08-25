using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Reports
{
    public class ReportDto
    {
        public long Id { get; set; }

        public DateTime ReportDate { get; set; }

        public int PlaceId { get; set; }

        public string PlaceName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public decimal TipAmount { get; set; }

        public decimal Total { get; set; }

        public bool TipStatus { get; set; }
    }
}
