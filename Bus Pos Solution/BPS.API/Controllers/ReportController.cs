using BPS.Application.DTOs.Reports;
using BPS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportController: ControllerBase
    {
        private readonly IReportService
            _reportService;

        private readonly IReportPdfGenerator _pdfGenerator;


        public ReportController(
            IReportService reportService, IReportPdfGenerator pdfGenerator)
        {
            _reportService =
                reportService;
            _pdfGenerator = pdfGenerator;
        }


        [HttpGet("get")]
        public async Task<IActionResult> Get(
            [FromQuery] ReportFilterDto filter)
        {
            var result =
                await _reportService
                    .GetAsync(filter);


            return Ok(result);
        }

        [HttpGet("get-paged")]
        public async Task<IActionResult> GetPaged([FromQuery] ReportFilterDto filter)
        {
            var result =
                await _reportService
                    .GetPagedAsync(filter);

            return Ok(result);
        }

        [HttpGet("export-pdf")]
        public async Task<IActionResult> ExportPdf([FromQuery] ReportFilterDto filter,CancellationToken cancellationToken)
        {
            var reports = (await _reportService.GetAsync(filter))
                .ToArray();

            var pdf = await _pdfGenerator.GenerateAsync(
                reports,
                filter,
                cancellationToken);

            return File(
                pdf,
                "application/pdf",
                $"trip-report-{DateTime.UtcNow:yyyyMMdd-HHmmss}.pdf");
        }
    }
}
