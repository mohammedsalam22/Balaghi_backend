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

    public ComplaintsController(
        SubmitComplaintService submitService,
        IComplaintDao complaintDao,
        IFileUploadService fileUploadService)
    {
        _submitService = submitService;
        _complaintDao = complaintDao;
        _fileUploadService = fileUploadService;
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
}