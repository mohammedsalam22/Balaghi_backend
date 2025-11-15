using Application.DTOs;
using Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InternetApplications.Extensions;
using System.Security.Claims;

namespace InternetApplications.Controllers
{
    [ApiController]
    [Route("api/complaints")]
    public class ComplaintsController(
        CreateComplaintService createComplaintService,
        GetComplaintsService getComplaintsService,
        UpdateComplaintStatusService updateComplaintStatusService) : ControllerBase
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComplaint(
            [FromBody] CreateComplaintRequest request,
            CancellationToken ct = default)
        {
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                var response = await createComplaintService.ExecuteAsync(request, userId, ct);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the complaint. Please try again later." });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetComplaints(CancellationToken ct = default)
        {
            try
            {
                var userId = User.GetUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized(new { message = "Invalid user token" });
                }

                // Check if user is Admin from JWT token claims
                var isAdmin = User.IsInRole("Admin");

                var response = await getComplaintsService.ExecuteAsync(userId, isAdmin, ct);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while retrieving complaints. Please try again later." });
            }
        }

        [HttpPut("{complaintId}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateComplaintStatus(
            Guid complaintId,
            [FromBody] UpdateComplaintStatusRequest request,
            CancellationToken ct = default)
        {
            try
            {
                // Check if user is Admin
                if (!User.IsInRole("Admin"))
                {
                    return StatusCode(403, new { message = "Only administrators can update complaint status" });
                }

                var response = await updateComplaintStatusService.ExecuteAsync(complaintId, request.Status, ct);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the complaint status. Please try again later." });
            }
        }
    }
}

