using Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRefreshTokenDao
    {
        Task AddAsync(RefreshToken token, CancellationToken ct = default);
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
        Task RevokeAsync(RefreshToken token, CancellationToken ct = default);
        void Revoke(RefreshToken token);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}