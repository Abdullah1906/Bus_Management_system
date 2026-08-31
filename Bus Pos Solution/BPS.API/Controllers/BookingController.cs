using BPS.Application.DTOs.Bookings;
using BPS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPS.API.Controllers
{
    [ApiController]
    [Route("api/v1/bookings")]
    [Authorize(Roles = "Customer")]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _service;

        public BookingController(
            IBookingService service)
        {
            _service = service;
        }

        [HttpPost("lock-seats")]
        public async Task<IActionResult> LockSeats(
            [FromBody] LockSeatsDto dto)
        {
            var result =
                await _service.LockSeatsAsync(dto);

            return Ok(result);
        }

        [HttpPost("confirm")]
        public async Task<IActionResult> Confirm(
        [FromBody] ConfirmBookingDto dto)
        {
            var result =
                await _service
                    .ConfirmBookingAsync(dto);

            return Ok(result);
        }
    }
}
