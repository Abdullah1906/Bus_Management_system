using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.DTOs.Reports
{
    public class ReportFilterDto
    {
        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int? PlaceId { get; set; }

        public string? Period { get; set; }
    }
}
