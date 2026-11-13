using BPS.Application.DTOs.StaffPayroll;
using BPS.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPS.API.Controllers
{
    [ApiController]
    [Route("api/v1/admin/staff")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffPayrollRepository _repository;
        private readonly IValidator<CreateStaffRequest> _validator;

        public StaffController(
            IStaffPayrollRepository repository,
            IValidator<CreateStaffRequest> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        // POST: api/v1/staff
        [HttpPost]
        public async Task<IActionResult> CreateStaff(
            [FromBody] CreateStaffRequest request,
            CancellationToken cancellationToken)
        {
            var validation = await _validator.ValidateAsync(
                request, cancellationToken);

            if (!validation.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed.",
                    errors = validation.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            var staffId = await _repository.CreateStaffAsync(
                request, cancellationToken);

            return Created(
                $"/api/v1/staff/{staffId}",
                new
                {
                    staffId,
                    message = "Staff created successfully."
                });
        }

        // GET: api/v1/staff
        [HttpGet]
        public async Task<IActionResult> GetStaffList(
            [FromQuery] StaffListRequest request,
            CancellationToken cancellationToken)
        {
            if (request.Page < 1 || request.PageSize < 1 ||
                request.PageSize > 100)
            {
                return BadRequest(new
                {
                    message = "Invalid pagination parameters."
                });
            }

            var result = await _repository.GetStaffListAsync(
                request, cancellationToken);

            return Ok(result);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStaffById(
            int id,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid Staff ID." });

            var staff = await _repository.GetStaffByIdAsync(
                id, cancellationToken);

            if (staff == null)
                return NotFound(new { message = "Staff not found." });

            return Ok(staff);
        }


        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStaff(
            int id,
            [FromBody] UpdateStaffRequest request,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid Staff ID." });

            var validation = await _validator.ValidateAsync(
                request, cancellationToken);

            if (!validation.IsValid)
            {
                return BadRequest(new
                {
                    message = "Validation failed.",
                    errors = validation.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        error = e.ErrorMessage
                    })
                });
            }

            await _repository.UpdateStaffAsync(
                id, request, cancellationToken);

            return Ok(new
            {
                staffId = id,
                message = "Staff updated successfully."
            });
        }



        [HttpPatch("{id:int}/status")]
        public async Task<IActionResult> UpdateStaffStatus(
            int id,
            [FromBody] UpdateStaffStatusRequest request,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid Staff ID." });

            var validStatuses = new[] { "Active", "Inactive" };

            if (!validStatuses.Contains(request.Status))
            {
                return BadRequest(new
                {
                    message = "Status must be Active or Inactive."
                });
            }

            await _repository.UpdateStaffStatusAsync(
                id,
                request.Status,
                cancellationToken);

            return Ok(new
            {
                staffId = id,
                status = request.Status,
                message = "Staff status updated successfully."
            });
        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStaff(
            int id,
            CancellationToken cancellationToken)
        {
            if (id <= 0)
                return BadRequest(new { message = "Invalid Staff ID." });

            await _repository.DeactivateStaffAsync(
                id, cancellationToken);

            return Ok(new
            {
                staffId = id,
                status = "Inactive",
                message = "Staff deactivated successfully."
            });
        }


    }
}
