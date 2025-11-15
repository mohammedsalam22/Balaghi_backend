using Domain.Common;

namespace Domain.Entities
{
    public class Complaint : BaseEntity
    {
        public string Type { get; set; } = null!;
        public string AssignedPart { get; set; } = null!; 
        public string Location { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int ComplaintNumber { get; private set; }
        public ComplaintStatus Status { get; set; } = ComplaintStatus.New;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        private Complaint() { } 

        public Complaint(string type, string assignedPart, string location, string description, Guid userId)
        {
            Type = type;
            AssignedPart = assignedPart;
            Location = location;
            Description = description;
            UserId = userId;
            ComplaintNumber = GenerateComplaintNumber();
            Status = ComplaintStatus.New;
        }

        private static int GenerateComplaintNumber()
        {
            var random = new Random();
            return random.Next(100000, 999999); 
        }

        // Method to regenerate the number if needed (for uniqueness checking)
        public void RegenerateComplaintNumber()
        {
            ComplaintNumber = GenerateComplaintNumber();
        }
    }
}

