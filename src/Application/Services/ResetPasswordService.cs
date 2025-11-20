using Application.DTOs;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public sealed class ResetPasswordService
    {
        private readonly IUserDao _userDao;
        private readonly IPasswordResetDao _resetDao;
        private readonly IPasswordHasher _hasher;

        public ResetPasswordService(
            IUserDao userDao,
            IPasswordResetDao resetDao,
            IPasswordHasher hasher)
        {
            _userDao = userDao;
            _resetDao = resetDao;
            _hasher = hasher;
        }

        public async Task ExecuteAsync(ResetPasswordRequest request, CancellationToken ct)
        {
            if (request.NewPassword != request.ConfirmPassword)
                throw new ArgumentException("Passwords do not match");
            var resetToken = await _resetDao.GetByTokenAsync(request.Token, ct)
                ?? throw new UnauthorizedAccessException("Invalid or expired reset link");
            if (!resetToken.IsValid())
                throw new UnauthorizedAccessException("Link expired or already used");
            var user = await _userDao.GetByIdWithRolesAsync(resetToken.UserId)
                ?? throw new UserNotFoundException("User not found");
            user.PasswordHash = _hasher.Hash(request.NewPassword);
            _resetDao.MarkAsUsed(resetToken);
        }
    }
}