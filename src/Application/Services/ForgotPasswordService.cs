using Application.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using BCrypt.Net;
namespace Application.Services
{
    public sealed class ForgotPasswordService
    {
        private readonly IUserDao _userDao;
        private readonly IPasswordResetDao _resetDao;
        private readonly IEmailService _emailService;

        public ForgotPasswordService(
            IUserDao userDao,
            IPasswordResetDao resetDao,
            IEmailService emailService)
        {
            _userDao = userDao;
            _resetDao = resetDao;
            _emailService = emailService;
        }

        public async Task ExecuteAsync(ForgotPasswordRequest request, CancellationToken ct)
        {
            var user = await _userDao.GetByEmailWithOtpsAsync(request.Email, ct)
                ?? throw new UserNotFoundException("Email not registered");

            if (!user.IsEmailVerified)
                throw new UnauthorizedAccessException("Email address not confirmed");

            var token = Guid.NewGuid().ToString("N");
            var tokenHash = BCrypt.Net.BCrypt.HashPassword(token);
            var resetToken = new PasswordResetToken(user.Id, tokenHash, 15);
            await _resetDao.AddAsync(resetToken, ct);
            var resetLink = $"https://localhost:7001/api/auth/reset-password?token={token}&email={Uri.EscapeDataString(user.Email)}";
            await _emailService.SendEmailAsync(
                user.Email,
                "Reset Your Password",
                $"Click the link to reset your password (valid for 15 minutes):\n\n{resetLink}");
        }
    }
}