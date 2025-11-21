using Domain.Common;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class Complaint : BaseEntity
    {
        public string TrackingNumber { get; private set; } = null!;
        public Guid CitizenId { get; private set; }
        public User Citizen { get; private set; } = null!;

        public Guid AgencyId { get; private set; }
        public GovernmentAgency Agency { get; private set; } = null!;

        public string ComplaintType { get; private set; } = null!;
        public string Description { get; private set; } = null!;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public ComplaintStatus Status { get; private set; } = ComplaintStatus.Pending;

        public List<ComplaintAttachment> Attachments { get; private set; } = new();

        private Complaint() { }

        public Complaint(Guid citizenId, Guid agencyId, string complaintType, string description)
        {
            CitizenId = citizenId;
            AgencyId = agencyId;
            ComplaintType = complaintType;
            Description = description;
            TrackingNumber = GenerateTrackingNumber();
        }

        private string GenerateTrackingNumber()
        {
            var year = DateTime.UtcNow.Year;
            var random = new Random();
            var number = random.Next(1, 999999).ToString("D6");
            return $"CMP-{year}-{number}";
        }

        public void AddAttachment(string fileName, string filePath, string contentType)
        {
            Attachments.Add(new ComplaintAttachment
            {
                FileName = fileName,
                FilePath = filePath,
                ContentType = contentType,
                ComplaintId = this.Id
            });
        }
    }

    public enum ComplaintStatus
    {
        Pending,
        UnderReview,
        InProgress,
        Resolved,
        Rejected
    }
}
