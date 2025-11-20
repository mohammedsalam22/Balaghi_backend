// مكان الملف: Infrastructure/Services/RefreshTokenService.cs

using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Services
{
    public sealed class RefreshTokenService
    {
        private readonly IRefreshTokenDao _refreshTokenDao;
        private readonly IUserDao _userDao;
        private readonly LoginService _loginService;

        public RefreshTokenService(
            IRefreshTokenDao refreshTokenDao,
            IUserDao userDao,
            LoginService loginService)
        {
            _refreshTokenDao = refreshTokenDao;
            _userDao = userDao;
            _loginService = loginService;
        }

        public async Task<LoginResponse> ExecuteAsync(string oldRefreshToken, CancellationToken ct = default)
        {
            var token = await _refreshTokenDao.GetByTokenAsync(oldRefreshToken, ct)
                ?? throw new UnauthorizedAccessException("Refresh Token not valid");

            if (token.IsExpired() || token.IsRevoked)
                throw new UnauthorizedAccessException("Refresh Token Expired or cancelled");

            var user = await _userDao.GetByIdWithRolesAsync(token.UserId)
                ?? throw new UnauthorizedAccessException("User not found");
            _refreshTokenDao.Revoke(token);
            var newRefreshToken = Guid.NewGuid().ToString("N");
            var newRefreshEntity = new RefreshToken(user.Id, newRefreshToken, 7);
            await _refreshTokenDao.AddAsync(newRefreshEntity, ct);
            var accessToken = _loginService.GenerateAccessToken(user);
            await _refreshTokenDao.SaveChangesAsync(ct);
            return new LoginResponse(accessToken, newRefreshToken, DateTime.UtcNow.AddHours(1));
        }
    }
}