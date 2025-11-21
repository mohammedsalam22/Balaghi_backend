using Application.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
namespace Application.Services;
public sealed class SubmitComplaintService
{
    private readonly IComplaintDao _complaintDao;
    private readonly IUserDao _userDao;
    private readonly IGovernmentAgencyDao _agencyDao;

    public SubmitComplaintService(
        IComplaintDao complaintDao,
        IUserDao userDao,
        IGovernmentAgencyDao agencyDao)
    {
        _complaintDao = complaintDao;
        _userDao = userDao;
        _agencyDao = agencyDao;
    }

    public async Task<SubmitComplaintResponse> ExecuteAsync(
        SubmitComplaintRequest request,
        Guid currentUserId,
        CancellationToken ct = default)
    {
        var citizen = await _userDao.GetByIdAsync(currentUserId)
    ?? throw new NotFoundException("User Not Found");

var agency = await _agencyDao.GetByIdAsync(request.AgencyId)
    ?? throw new NotFoundException("Agency Not found");
        var complaint = new Complaint(
            citizenId: currentUserId,
            agencyId: request.AgencyId,
            complaintType: request.ComplaintType,
            description: request.Description);
        if (request.Attachments != null && request.Attachments.Any())
        {
            foreach (var file in request.Attachments)
            {
                complaint.AddAttachment(
                    fileName: file.FileName,
                    filePath: file.FilePath,
                    contentType: file.ContentType);
            }
        }

        await _complaintDao.AddAsync(complaint, ct);
        await _complaintDao.SaveChangesAsync(ct);

        return new SubmitComplaintResponse
        {
            TrackingNumber = complaint.TrackingNumber,
            Message = "The complaint has been successfully submitted; you can track it using the tracking number",
            SubmittedAt = complaint.CreatedAt
        };
    }
}