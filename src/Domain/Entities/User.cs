using Domain.Common;
namespace Domain.Entities
{
    public class User : BaseEntity
    {
        public string FullName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string PasswordHash { get;  set; }
        public bool IsEmailVerified { get;  set; }
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public virtual ICollection<OtpCode> OtpCodes { get; init; } = new List<OtpCode>();
        public Guid? DepartmentId { get; set; }
        public Guid? GovernmentEntityId { get; private set; }
        public GovernmentEntity? GovernmentEntity { get; private set; }
        public void AssignToGovernmentEntity(Guid governmentEntityId)
        {
            GovernmentEntityId = governmentEntityId;
            
}
        private User() { }
        public User(string fullName, string email, string passwordHash)
        {
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            IsEmailVerified = false;
        }
        public void VerifyEmail() => IsEmailVerified = true;
    }
}