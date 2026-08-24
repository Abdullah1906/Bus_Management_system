using BPS.Application.DTOs.Trips;
using BPS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPS.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TripsController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripsController(
            ITripService tripService)
        {
            _tripService = tripService;
        }


        [HttpPost("create")]
        public async Task<IActionResult> Create(
            CreateTripDto dto)
        {
            var result =
                await _tripService
                    .CreateAsync(dto);

            return Ok(result);
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _tripService
                    .GetAllAsync();

            return Ok(result);
        }


        [HttpGet("get/{id:int}")]
        public async Task<IActionResult> GetById(
            long id)
        {
            var result =
                await _tripService
                    .GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("update/{id:long}")]
        public async Task<IActionResult> Update( long id, UpdateTripDto dto)
        {
            var result =
                await _tripService
                    .UpdateAsync(id, dto);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Trip not found."
                });
            }

            return Ok(result);
        }


        [HttpDelete("delete/{id:long}")]
        public async Task<IActionResult> Delete(
            long id)
        {
            var result =
                await _tripService
                    .DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Trip not found."
                });
            }

            return NoContent();
        }
    }
}
