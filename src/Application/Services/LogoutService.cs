using Domain.Interfaces;

namespace Application.Services
{
    public class LogoutService
    {
        private readonly IRefreshTokenDao _refreshTokenDao;

        public LogoutService(IRefreshTokenDao refreshTokenDao)
        {
            _refreshTokenDao = refreshTokenDao;
        }

        public async Task ExecuteAsync(string refreshToken, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return;

            var token = await _refreshTokenDao.GetByTokenAsync(refreshToken, ct);
            if (token != null && !token.IsRevoked && !token.IsExpired())
            {
                _refreshTokenDao.Revoke(token);
                await _refreshTokenDao.SaveChangesAsync(ct);
            }
        }
    }
}