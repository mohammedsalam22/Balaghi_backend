using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess
{
    public class RefreshTokenDao : IRefreshTokenDao
    {
        private readonly AppDbContext _context;

        public RefreshTokenDao(AppDbContext context)
        {
            _context = context;
        }
        public async Task RevokeAsync(RefreshToken token, CancellationToken ct = default)
        {
            token.Revoke();
            _context.Set<RefreshToken>().Update(token);
        }
        public async Task AddAsync(RefreshToken token, CancellationToken ct = default)
            => await _context.Set<RefreshToken>().AddAsync(token, ct);

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
            => await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow, ct);

        public void Revoke(RefreshToken token)
        {
            if (token == null) return;
            token.Revoke(); 
            _context.Set<RefreshToken>().Update(token);
        }
        public async Task SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
        public void Update(RefreshToken token)
        {
            if (token != null)
                _context.Set<RefreshToken>().Update(token);
        }
    }
}