using Application.DTOs;
using Domain.Exceptions;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Application.Services
{
    public sealed class ResetPasswordService
    {
        private readonly IUserRepository _userRepo;
        private readonly IPasswordResetRepository _resetRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _hasher;

        public ResetPasswordService(
            IUserRepository userRepo,
            IPasswordResetRepository resetRepo,
            IUnitOfWork unitOfWork,
            IPasswordHasher hasher
            )
        {
            _userRepo = userRepo;
            _resetRepo = resetRepo;
            _unitOfWork = unitOfWork;
            _hasher = hasher;
        }

        public async Task ExecuteAsync(ResetPasswordRequest request, CancellationToken ct)
        {
            if (request.NewPassword != request.ConfirmPassword)
                throw new ArgumentException("The password and its confirmation do not match");

            var resetToken = await _resetRepo.GetByTokenAsync(request.Token, ct)
                ?? throw new UnauthorizedAccessException("The link is invalid or expired");

            if (!resetToken.IsValid())
                throw new UnauthorizedAccessException("The link has expired or has already been used");

            var user = await _userRepo.GetByIdWithRolesAsync(resetToken.UserId)
                ?? throw new UserNotFoundException("user not found");

            user.PasswordHash = _hasher.Hash(request.NewPassword);
            _resetRepo.MarkAsUsed(resetToken);
            await _unitOfWork.SaveChangesAsync(ct);
        }

    }
}
