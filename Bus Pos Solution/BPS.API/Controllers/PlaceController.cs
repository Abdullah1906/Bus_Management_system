using BPS.Application.DTOs.Places;
using BPS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPS.API.Controllers
{
   
    [ApiController]
    [Route("api/[controller]")]
    public class PlaceController : ControllerBase
    {
        private readonly IPlaceService _placeService;

        public PlaceController(
            IPlaceService placeService)
        {
            _placeService = placeService;
        }


        // GET: api/place
        [HttpGet("getall")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var places =
                await _placeService.GetAllAsync();

            return Ok(places);
        }


        // GET: api/place/1
        [HttpGet("get/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(
            int id)
        {
            var place =
                await _placeService.GetByIdAsync(id);

            if (place == null)
            {
                return NotFound(new
                {
                    message = "Place not found."
                });
            }

            return Ok(place);
        }


        // POST: api/place
        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> Create(
            [FromBody] CreatePlaceDto dto)
        {
            try
            {
                var id =
                    await _placeService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id },
                    new
                    {
                        id,
                        message = "Place created successfully."
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // PUT: api/place/1
        [HttpPut("update/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdatePlaceDto dto)
        {
            try
            {
                var result =
                    await _placeService.UpdateAsync(
                        id,
                        dto);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Place not found."
                    });
                }

                return Ok(new
                {
                    message = "Place updated successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // DELETE: api/place/1
        [HttpDelete("delete/{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(
            int id)
        {
            var result =
                await _placeService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Place not found."
                });
            }

            return Ok(new
            {
                message = "Place deleted successfully."
            });
        }
    }
}
