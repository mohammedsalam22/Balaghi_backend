using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class PasswordResetToken:BaseEntity
    {
        public Guid UserId { get; private set; }
        public string TokenHash { get; private set; } = null!;
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTime? UsedAt { get; private set; }

        private PasswordResetToken() { }

        public PasswordResetToken(Guid userId, string tokenHash, int validMinutes = 15)
        {
            UserId = userId;
            TokenHash = tokenHash;
            ExpiresAt = DateTime.UtcNow.AddMinutes(validMinutes);
            IsUsed = false;
        }

        public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
        public bool IsValid() => !IsUsed && !IsExpired();

        public void MarkAsUsed()
        {
            IsUsed = true;
            UsedAt = DateTime.UtcNow;
        }
    }
}
