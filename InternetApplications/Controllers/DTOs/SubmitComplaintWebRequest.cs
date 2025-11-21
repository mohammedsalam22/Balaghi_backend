public class SubmitComplaintWebRequest
{
    public Guid AgencyId { get; set; }
    public string ComplaintType { get; set; } = null!;
    public string Description { get; set; } = null!;
    public List<IFormFile>? Attachments { get; set; }
}