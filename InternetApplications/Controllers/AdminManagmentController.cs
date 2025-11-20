using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Services;
using Application.DTOs;
using Domain.Exceptions;
namespace InternetApplications.Controllers;
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminManagementController : ControllerBase
{
    private readonly InviteEmployeeService _inviteService;
    private readonly CompleteEmployeeSetupService _completeSetupService;
    public AdminManagementController(
        InviteEmployeeService inviteService,
        CompleteEmployeeSetupService completeSetupService)
    {
        _inviteService = inviteService;
        _completeSetupService = completeSetupService;
    }
    [HttpPost("invite-employee")]
    public async Task<IActionResult> InviteEmployee([FromBody] InviteEmployeeRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var userId = await _inviteService.ExecuteAsync(
                agencyId: request.AgencyId,
                fullName: request.FullName,
                email: request.Email);

            return Ok(new
            {
                success = true,
                message = "The verification code was successfully sent to the employees email",
                userId
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
           return StatusCode(500, new
    {
        success = false,
        message = "try again later",
        error = ex.Message,
        stack = ex.StackTrace
    });
        }
    }
    [HttpPost("complete-setup")]
    [AllowAnonymous]
    public async Task<IActionResult> CompleteSetup([FromBody] CompleteSetupRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            await _completeSetupService.CompleteAsync(request.Code, request.NewPassword);

            return Ok(new
            {
                success = true,
                message = "The password has been successfully set you can now log in"
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { success = false, message = "try again later" });
        }
    }
}