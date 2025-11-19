using Application.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
namespace Application.Services
{
    public sealed class ForgotPasswordService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordResetRepository _resetRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        public ForgotPasswordService(
            IUserRepository userRepo,
            IPasswordResetRepository resetRepo,
            IUnitOfWork unitOfWork,
            IEmailService emailService)
        {
            _userRepo = userRepo;
            _resetRepo = resetRepo;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task ExecuteAsync(ForgotPasswordRequest request, CancellationToken ct)
        {
            var user = await _userRepo.GetByEmailWithOtpsAsync(request.Email, ct)
                ?? throw new UserNotFoundException("Email not registered");

            if (!user.IsEmailVerified)
                throw new UnauthorizedAccessException("Email address not confirmed");

            var token = Guid.NewGuid().ToString("N");
            var tokenHash = BCrypt.Net.BCrypt.HashPassword(token);

            var resetToken = new PasswordResetToken(user.Id, tokenHash, 15);
            await _resetRepo.AddAsync(resetToken, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            var resetLink = $"https://localhost:7001/api/auth/reset-password?token={token}&email={Uri.EscapeDataString(user.Email)}";

            await _emailService.SendEmailAsync(
                user.Email,
                "Reset password",
                $"((Click the link to reset your password(Valid for 15 minutes):\n{resetLink}"
            );
        }
    }
}