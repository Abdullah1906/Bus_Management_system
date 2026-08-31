using BPS.Application.DTOs.Trips;
using BPS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPS.API.Controllers
{
    [ApiController]
    [Route("api/v1/admin/trips")]
    [Authorize(Roles = "Admin")]
    public class AdminTripController
    : ControllerBase
    {
        private readonly ITripScheduleService _service;

        public AdminTripController(
            ITripScheduleService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateTripScheduleDto dto)
        {
            var result =
                await _service.CreateAsync(dto);

            return Ok(result);
        }
    }
}
