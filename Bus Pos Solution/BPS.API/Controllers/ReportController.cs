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


        public ReportController(
            IReportService reportService)
        {
            _reportService =
                reportService;
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
    }
}
