using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class LogoutService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepo;

        public LogoutService(IRefreshTokenRepository refreshTokenRepo)
        {
            _refreshTokenRepo = refreshTokenRepo;
        }

        public async Task ExecuteAsync(string refreshToken, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return;

            var token = await _refreshTokenRepo.GetByTokenAsync(refreshToken, ct);

            if (token != null && !token.IsRevoked && !token.IsExpired())
            {
                _refreshTokenRepo.Revoke(token);
                await _refreshTokenRepo.SaveChangesAsync(ct);
            }
        }
    }
}
