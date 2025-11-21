using Application.DTOs;
using Application.Services;
using Domain.Interfaces;
using InternetApplications.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InternetApplications.Controllers;

[Route("api/complaints")]
[ApiController]
public class ComplaintsController : ControllerBase
{
    private readonly SubmitComplaintService _submitService;
    private readonly IComplaintDao _complaintDao;
    private readonly IFileUploadService _fileUploadService;
    private readonly IUserDao _userDao;
    public ComplaintsController(
        SubmitComplaintService submitService,
        IComplaintDao complaintDao,
        IFileUploadService fileUploadService,
        IUserDao userDao)
    {
        _submitService = submitService;
        _complaintDao = complaintDao;
        _fileUploadService = fileUploadService;
        _userDao = userDao;
    }
    [HttpPost("submit")]
    [Authorize(Roles = "User")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> Submit(
        [FromForm] Guid agencyId,
        [FromForm] string complaintType,
        [FromForm] string description,
        [FromForm] List<IFormFile>? attachments)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var uploadedFiles = new List<UploadedFileDto>();
        if (attachments?.Count > 0)
        {
            uploadedFiles = await _fileUploadService.UploadAsync(attachments, "complaints");
        }

        var request = new SubmitComplaintRequest
        {
            AgencyId = agencyId,
            ComplaintType = complaintType,
            Description = description,
            Attachments = uploadedFiles
        };

        var result = await _submitService.ExecuteAsync(request, userId);

        return Ok(new
        {
            success = true,
            message = result.Message,
            data = new
            {
                result.TrackingNumber,
                result.SubmittedAt,
                attachments = uploadedFiles.Select(f => new { f.FileName, Url = f.FilePath })
            }
        });
    }
    [HttpGet("track/{trackingNumber}")]
    [AllowAnonymous]
    public async Task<IActionResult> Track(string trackingNumber)
    {
        var complaint = await _complaintDao.GetByTrackingNumberAsync(trackingNumber);
        if (complaint == null)
            return NotFound(new { success = false, message = "The tracking number is incorrect" });
        return Ok(new
        {
            success = true,
            data = new
            {
                complaint.TrackingNumber,
                complaint.ComplaintType,
                Agency = complaint.Agency.Name,
                complaint.Status,
                complaint.CreatedAt,
                Attachments = complaint.Attachments.Select(a => new
                {
                    a.FileName,
                    Url = a.FilePath
                })
            }
        });
    }
    [HttpGet("my-agency")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> GetMyAgencyComplaints()
    {
        var employeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var employee = await _userDao.GetByIdAsync(employeeId);
        if (employee?.GovernmentAgencyId == null)
            return Forbid();
        var complaints = await _complaintDao.GetByAgencyIdAsync(employee.GovernmentAgencyId.Value);
        var result = complaints.Select(c => new
        {
            c.Id,
            c.TrackingNumber,
            c.ComplaintType,
            c.Description,
            CitizenName = c.Citizen?.FullName ?? "Not found",
            c.Status,
            c.CreatedAt,
            AttachmentsCount = c.Attachments.Count
        });

        return Ok(new { success = true, data = result });
    }
    [HttpPatch("{id:guid}/status")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateComplaintStatusRequest request)
    {
        var employeeId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var employee = await _userDao.GetByIdAsync(employeeId);
        if (employee?.GovernmentAgencyId == null)
            return Forbid();

        var complaint = await _complaintDao.GetByIdWithDetailsAsync(id);
        if (complaint == null)
            return NotFound(new { success = false, message = "Complaint Not found" });

        if (complaint.AgencyId != employee.GovernmentAgencyId)
            return Forbid(); 

        complaint.Status = request.Status;
        await _complaintDao.SaveChangesAsync();

        return Ok(new
        {
            success = true,
            message = $" Status update to {request.Status}",
            data = new { complaint.TrackingNumber, complaint.Status }
        });
    }
}