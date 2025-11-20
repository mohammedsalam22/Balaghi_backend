// src/Domain/Entities/PasswordSetupOtp.cs
using Domain.Common;

namespace Domain.Entities;

public class PasswordSetupOtp : BaseEntity
{
   public Guid UserId { get; set; } 
    public User User { get; private set; } = null!;

    public string Code { get; private set; } = null!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }

    private PasswordSetupOtp() { }

    public PasswordSetupOtp(Guid userId, string code)
    {
       UserId = userId;
        Code = code;
        ExpiresAt = DateTime.UtcNow.AddMinutes(15);
        IsUsed = false;
    }

    public bool IsValid() => !IsUsed && DateTime.UtcNow <= ExpiresAt;
    public void MarkAsUsed() => IsUsed = true;
}