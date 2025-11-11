using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class OtpCode:BaseEntity
    {
        public Guid UserId { get; private set; }
        public virtual User User { get; private set; } = null!;
        public string Code { get; private set; } = null!;
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        private OtpCode() { }
        public OtpCode(Guid userId, string code, int validMinutes = 5)
        {
            UserId = userId;
            Code = code;
            ExpiresAt = DateTime.UtcNow.AddMinutes(validMinutes);
            IsUsed = false;
        }
        public bool IsValid(string code)
        => !IsUsed && Code == code && DateTime.UtcNow <= ExpiresAt;
        public void MarkAsUsed() => IsUsed = true;
    }
}
