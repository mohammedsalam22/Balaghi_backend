using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Exceptions;
namespace Application.Services
{
    public class RegisterUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRoleRepository _roleRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _hasher;
        private readonly IEmailService _emailService;

        public RegisterUserService(
            IUserRepository userRepo,
            IRoleRepository roleRepo,
            IUnitOfWork unitOfWork,
            IPasswordHasher hasher,
            IEmailService emailService)
        {
            _userRepo = userRepo;
            _roleRepo = roleRepo;
            _unitOfWork = unitOfWork;
            _hasher = hasher;
            _emailService = emailService;
        }

        public async Task<RegisterResponse> ExecuteAsync(RegisterRequest request, CancellationToken ct = default)
        {
            if (await _userRepo.ExistsByEmailAsync(request.Email, ct))
                throw new UserAlreadyExistsException(request.Email);
            var user = new User(request.FullName, request.Email, _hasher.Hash(request.Password));
            var defaultRole = await _roleRepo.GetByNameAsync("User", ct)
                ?? throw new InvalidOperationException("Role 'User' Not found  ");

            user.UserRoles.Add(new UserRole(user.Id, defaultRole.Id));
            await _userRepo.AddAsync(user, ct);
            var otpCode = Random.Shared.Next(100000, 999999).ToString("D6");
            var otp = new OtpCode(user.Id, otpCode, 5);
            await _userRepo.AddOtpAsync(otp, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            await _emailService.SendOtpAsync(request.Email, request.FullName, otpCode, ct);
            return new RegisterResponse("Registration was successful A verification code has been sent to your email");
        }
    }
}
