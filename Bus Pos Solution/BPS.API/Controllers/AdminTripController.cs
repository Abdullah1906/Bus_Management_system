using BPS.Application.DTOs.TripSchedules;
using BPS.Application.DTOs.TripSearch;
using BPS.Application.Interfaces;
using BPS.Application.Services;
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
        {
            var result =
                await _service.GetAllAsync(
                    cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id,CancellationToken cancellationToken)
        {
            var result =
                await _service.GetByIdAsync(
                    id,
                    cancellationToken);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id,[FromBody] UpdateTripScheduleDto dto)
        {
            var result =
                await _service.UpdateAsync(id, dto);

            if (!result)
                return NotFound();

            return Ok(result);
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var result =
                await _service.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok(result);
        }
        [HttpPatch("{id:long}/status")]
        public async Task<IActionResult> ChangeStatus(long id, [FromBody] bool isActive)
        {
            var result =
                await _service.ChangeStatusAsync(
                    id,
                     isActive);

            if (!result)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
        [FromQuery] string fromPlace,
        [FromQuery] string toPlace,
        [FromQuery] DateTime tripDate)
        {
            var request = new TripSearchDto
            {
                FromPlace = fromPlace,
                ToPlace = toPlace,
                TripDate = tripDate
            };

            var result = await _service.SearchAsync(request);

            return Ok(result);
        }
    }
}
