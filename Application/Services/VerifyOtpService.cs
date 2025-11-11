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
    public sealed class VerifyOtpService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public VerifyOtpService(IUserRepository userRepo, IUnitOfWork unitOfWork)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<VerifyOtpResponse> ExecuteAsync(VerifyOtpRequest request, CancellationToken ct = default)
        {
            var user = await _userRepo.GetByEmailWithOtpsAsync(request.Email, ct)
                ?? throw new UserNotFoundException(request.Email);

            if (user.IsEmailVerified)
                return new VerifyOtpResponse("The email address is pre-confirmed");
            var otp = user.OtpCodes
                .Where(o => !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            if (otp is null || !otp.IsValid(request.Code))
                throw new OtpInvalidException();
            user.VerifyEmail();
            _userRepo.MarkOtpAsUsed(otp);
            await _unitOfWork.SaveChangesAsync(ct);

            return new VerifyOtpResponse("Your email has been successfully confirmed You can now log in");
        }
    }
}