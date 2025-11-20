using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess
{
    public class PasswordResetDao : IPasswordResetDao
    {
        private readonly AppDbContext _context;

        public PasswordResetDao(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PasswordResetToken token, CancellationToken ct = default)
            => await _context.Set<PasswordResetToken>().AddAsync(token, ct);

        public async Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default)
            => await _context.Set<PasswordResetToken>()
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);

        public async Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            var tokens = await _context.Set<PasswordResetToken>().ToListAsync(ct);
            return tokens.FirstOrDefault(t => BCrypt.Net.BCrypt.Verify(token, t.TokenHash));
        }

        public void MarkAsUsed(PasswordResetToken token)
        {
            token.MarkAsUsed();
            _context.Set<PasswordResetToken>().Update(token);
        }
    }
}