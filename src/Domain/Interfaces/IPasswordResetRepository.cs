using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPasswordResetRepository
    {
        Task AddAsync(PasswordResetToken token, CancellationToken ct);
        Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct);
        void MarkAsUsed(PasswordResetToken token);
        Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken ct);
    }
}
