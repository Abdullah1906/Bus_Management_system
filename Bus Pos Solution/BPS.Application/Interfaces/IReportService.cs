using BPS.Application.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IReportService
    {
        Task<IEnumerable<ReportDto>> GetAsync(
            ReportFilterDto filter);
    }
}
