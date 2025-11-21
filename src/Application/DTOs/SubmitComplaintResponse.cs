public class SubmitComplaintResponse
{
    public string TrackingNumber { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTime SubmittedAt { get; set; }
}