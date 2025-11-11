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
    public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
    {
        private readonly AppDbContext _context = context;

        public async Task AddAsync(RefreshToken token, CancellationToken ct)
            => await _context.Set<RefreshToken>().AddAsync(token, ct);

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct)
            => await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(t => t.Token == token, ct);

        public async Task RevokeAsync(RefreshToken token, CancellationToken ct)
        {
            token.Revoke();
            _context.Set<RefreshToken>().Update(token);
        }
        public void Revoke(RefreshToken token)
        {
            token.Revoke();
            _context.Set<RefreshToken>().Update(token);
        }
        public async Task SaveChangesAsync(CancellationToken ct)
        => await _context.SaveChangesAsync(ct);
    }
}