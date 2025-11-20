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
          public Guid? GovernmentAgencyId { get; set; }
    public GovernmentAgency? GovernmentAgency { get; set; }
        public ICollection<PasswordSetupOtp> PasswordSetupOtps { get; set; } = new List<PasswordSetupOtp>();
        private User() { }
        public User(string fullName, string email, string passwordHash)
        {
            FullName = fullName;
            Email = email;
            PasswordHash = passwordHash;
            IsEmailVerified = false;
        }
        public User(string fullName, string email, string passwordHash = "", bool isEmailVerified = false)
{
    FullName = fullName;
    Email = email;
    PasswordHash = passwordHash;
    IsEmailVerified = isEmailVerified;
}
        
        public void VerifyEmail() => IsEmailVerified = true;
        public void AssignToAgency(Guid agencyId) => GovernmentAgencyId = agencyId;
    public void SetPassword(string hash) => PasswordHash = hash;
    public bool HasPassword() => !string.IsNullOrWhiteSpace(PasswordHash);
    }
}