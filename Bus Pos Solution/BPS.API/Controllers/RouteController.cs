using BPS.Application.DTOs.Routes;
using BPS.Application.Interfaces;
using BPS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPS.API.Controllers
{
    [ApiController]
    [Route("api/v1/admin/routes")]
    [Authorize(Roles = "Admin")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RouteController(
            IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result =
                await _routeService.GetAllAsync();

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(
            int id)
        {
            var result =
                await _routeService.GetByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateRouteDto dto)
        {
            var result =
                await _routeService.CreateAsync(dto);

            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateRouteDto dto)
        {
            var result =
                await _routeService.UpdateAsync(
                    id,
                    dto);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message =
                    "Route updated successfully."
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(
            int id)
        {
            var result =
                await _routeService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message =
                    "Route deactivated successfully."
            });
        }

        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> ChangeStatus(
            int id,
            [FromBody] bool isActive)
        {
            var result =
                await _routeService.ChangeStatusAsync(
                    id,
                    isActive);

            if (!result)
                return NotFound();

            return Ok(new
            {
                message = isActive
                    ? "Route activated successfully."
                    : "Route deactivated successfully."
            });
        }
    }
}
