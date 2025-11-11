using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken token, CancellationToken ct);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct);
        Task RevokeAsync(RefreshToken token, CancellationToken ct);
        void Revoke(RefreshToken token);
        Task SaveChangesAsync(CancellationToken ct);

    }
}
