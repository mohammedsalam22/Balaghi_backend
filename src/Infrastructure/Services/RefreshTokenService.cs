using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public sealed class RefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly LoginService _loginService;

        public RefreshTokenService(
            IRefreshTokenRepository refreshRepo,
            IUserRepository userRepo,
            IUnitOfWork unitOfWork,
            LoginService loginService)
        {
            _refreshRepo = refreshRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
            _loginService = loginService;
        }
        public async Task<LoginResponse> ExecuteAsync(string oldRefreshToken, CancellationToken ct)
        {
            var token = await _refreshRepo.GetByTokenAsync(oldRefreshToken, ct)
                ?? throw new UnauthorizedAccessException("Refresh Token not valid");
            if (token.IsExpired() || token.IsRevoked)
                throw new UnauthorizedAccessException("Refresh Token Expired or cancelled");
            var user = await _userRepo.GetByIdWithRolesAsync(token.UserId)
                ?? throw new UnauthorizedAccessException("User not found");
            await _refreshRepo.RevokeAsync(token, ct);
            var newRefreshToken = Guid.NewGuid().ToString();
            var newRefreshEntity = new RefreshToken(user.Id, newRefreshToken, 7);
            await _refreshRepo.AddAsync(newRefreshEntity, ct);
            var accessToken = _loginService.GenerateAccessToken(user);
            await _unitOfWork.SaveChangesAsync(ct);
            return new LoginResponse(accessToken, newRefreshToken, DateTime.UtcNow.AddHours(1));
        }
    }
}
