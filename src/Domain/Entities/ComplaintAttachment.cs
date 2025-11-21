using Domain.Entities;

public class ComplaintAttachment
    {
        public Guid Id { get; set; }
        public Guid ComplaintId { get; set; }
        public Complaint Complaint { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }