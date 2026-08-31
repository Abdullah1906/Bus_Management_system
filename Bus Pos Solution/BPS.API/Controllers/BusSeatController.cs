using BPS.Application.DTOs.BusSeats;
using BPS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPS.API.Controllers
{
    [ApiController]
    [Route("api/v1/admin/buses/{busId:int}/seats")]
    [Authorize(Roles = "Admin")]
    public class BusSeatController : ControllerBase
    {
        private readonly BusSeatService _service;

        public BusSeatController(
            BusSeatService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetSeats(
            int busId)
        {
            var result =
                await _service
                    .GetByBusIdAsync(busId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            int busId,
            [FromBody] CreateBusSeatDto dto)
        {
            var result =
                await _service.CreateAsync(
                    busId,
                    dto);

            return Ok(result);
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(
            int busId,
            long id,
            [FromBody] UpdateBusSeatDto dto)
        {
            var result =
                await _service.UpdateAsync(
                    id,
                    dto);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message =
                    "Seat updated successfully."
            });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(
            int busId,
            long id)
        {
            var result =
                await _service.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message =
                    "Seat deleted successfully."
            });
        }

        [HttpPatch("{id:long}/status")]
        public async Task<IActionResult> ChangeStatus(
            int busId,
            long id,
            [FromBody] bool isActive)
        {
            var result =
                await _service.ChangeStatusAsync(
                    id,
                    isActive);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message = isActive
                    ? "Seat activated successfully."
                    : "Seat deactivated successfully."
            });
        }
    }
}
