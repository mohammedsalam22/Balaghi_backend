using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class PasswordResetRepository(AppDbContext context) : IPasswordResetRepository
    {
        private readonly AppDbContext _context = context
            ?? throw new ArgumentNullException(nameof(context));

        public async Task AddAsync(PasswordResetToken token, CancellationToken ct)
            => await _context.Set<PasswordResetToken>().AddAsync(token, ct);

        public async Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct)
            => await _context.Set<PasswordResetToken>()
                .FirstOrDefaultAsync(t => t.TokenHash == tokenHash, ct);
        public async Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken ct)
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