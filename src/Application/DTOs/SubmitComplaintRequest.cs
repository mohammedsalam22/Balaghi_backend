
namespace Application.DTOs;
public class SubmitComplaintRequest
{
    public Guid AgencyId { get; set; }
    public string ComplaintType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<UploadedFileDto>? Attachments { get; set; } 
}