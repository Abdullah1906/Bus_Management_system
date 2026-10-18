using BPS.Application.DTOs.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPS.Application.Interfaces
{
    public interface IReportPdfGenerator
    {
        Task<byte[]> GenerateAsync(
            IReadOnlyCollection<ReportDto> reports,
            ReportFilterDto filter,
            CancellationToken cancellationToken = default);
    }
}
