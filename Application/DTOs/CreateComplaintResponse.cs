namespace Application.DTOs
{
    public record CreateComplaintResponse(string Message, int ComplaintNumber, Guid ComplaintId, bool Success = true);
}

