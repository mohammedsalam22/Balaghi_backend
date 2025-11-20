using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Services;
using Domain.Exceptions;
using Application.DTOs;

namespace InternetApplications.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class GovernmentAgencyController : ControllerBase
    {
        private readonly GovernorateAgencyService _service;

        public GovernmentAgencyController(GovernorateAgencyService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var agencies = await _service.GetAllAsync();
                var response = new GovernormentAgency<IEnumerable<GovernmentAgencyDto>>(
                    "Agencies retrieved successfully",
                    true,
                    agencies
                );
                return Ok(response);
            }
            catch (Exception)
            {
                var response = new GovernormentAgency<object>(
                    "Failed to retrieve agencies",
                    false
                );
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var agency = await _service.GetByIdAsync(id);
                var response = new GovernormentAgency<GovernmentAgencyDto>(
                    "Agency retrieved successfully",
                    true,
                    agency
                );
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                var response = new GovernormentAgency<object>(
                    ex.Message,
                    false
                );
                return NotFound(response);
            }
            catch (Exception)
            {
                var response = new GovernormentAgency<object>(
                    "Failed to retrieve the agency",
                    false
                );
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string name)
        {
            try
            {
                var agency = await _service.CreateAsync(name);
                var response = new GovernormentAgency<GovernmentAgencyDto>(
                    "Agency created successfully",
                    true,
                    agency
                );
                return CreatedAtAction(nameof(GetById), new { id = agency.Id }, response);
            }
            catch (Exception)
            {
                var response = new GovernormentAgency<object>(
                    "Failed to create the agency",
                    false
                );
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] string name)
        {
            try
            {
                await _service.UpdateAsync(id, name);
                var response = new GovernormentAgency<object>(
                    "Agency updated successfully",
                    true
                );
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                var response = new GovernormentAgency<object>(
                    ex.Message,
                    false
                );
                return NotFound(response);
            }
            catch (Exception)
            {
                var response = new GovernormentAgency<object>(
                    "Failed to update the agency",
                    false
                );
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                var response = new GovernormentAgency<object>(
                    "Agency deleted successfully",
                    true
                );
                return Ok(response);
            }
            catch (NotFoundException ex)
            {
                var response = new GovernormentAgency<object>(
                    ex.Message,
                    false
                );
                return NotFound(response);
            }
            catch (Exception)
            {
                var response = new GovernormentAgency<object>(
                    "Failed to delete the agency",
                    false
                );
                return StatusCode(500, response);
            }
        }
    }
}
