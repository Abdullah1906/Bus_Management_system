using BPS.Application.DTOs.Buses;
using BPS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BPS.API.Controllers
{
    [ApiController]
    [Route("api/v1/admin/buses")]
    [Authorize(Roles = "Admin")]
    public class BusController : ControllerBase
    {
        private readonly IBusService _busService;

        public BusController(
            IBusService busService)
        {
            _busService = busService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateBusDto dto)
        {
            var email =
                User.FindFirstValue(
                    ClaimTypes.Email);

            var result =
                await _busService.CreateAsync(
                    dto,
                    email);

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _busService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var result =
                await _busService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateBusDto dto)
        {
            var email =
                User.FindFirstValue(
                    ClaimTypes.Email);

            var result =
                await _busService.UpdateAsync(
                    id,
                    dto,
                    email);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message = "Bus updated successfully."
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            var result =
                await _busService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message = "Bus deleted successfully."
            });
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus(
            int id,
            [FromBody] bool isActive)
        {
            var email =
                User.FindFirstValue(
                    ClaimTypes.Email);

            var result =
                await _busService.ChangeStatusAsync(
                    id,
                    isActive,
                    email);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message =
                    isActive
                        ? "Bus activated successfully."
                        : "Bus deactivated successfully."
            });
        }
    }
}
