using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class RefreshToken:BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Token { get; private set; } = null!;
        public DateTime ExpiresAt { get; private set; }
        public bool IsRevoked { get; private set; }
        public DateTime? RevokedAt { get; private set; }
        private RefreshToken() { }

        public RefreshToken(Guid userId, string token, int validDays = 7)
        {
            UserId = userId;
            Token = token;
            ExpiresAt = DateTime.UtcNow.AddDays(validDays);
            IsRevoked = false;
        }
        public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
        public void Revoke()
        {
            IsRevoked = true;
            RevokedAt = DateTime.UtcNow;
        }
    }
}
