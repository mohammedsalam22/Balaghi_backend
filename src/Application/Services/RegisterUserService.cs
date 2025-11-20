using Application.DTOs;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public class RegisterUserService
    {
        private readonly IUserDao _userDao;
        private readonly IRoleDao _roleDao;
        private readonly IPasswordHasher _hasher;
        private readonly IEmailService _emailService;

        public RegisterUserService(
            IUserDao userDao,
            IRoleDao roleDao,
            IPasswordHasher hasher,
            IEmailService emailService)
        {
            _userDao = userDao;
            _roleDao = roleDao;
            _hasher = hasher;
            _emailService = emailService;
        }

        public async Task<RegisterResponse> ExecuteAsync(RegisterRequest request, CancellationToken ct = default)
        {
            if (await _userDao.ExistsByEmailAsync(request.Email, ct))
                throw new UserAlreadyExistsException(request.Email);
            var user = new User(request.FullName, request.Email, _hasher.Hash(request.Password))
            {
                Id = Guid.NewGuid()
            };
            var defaultRole = await _roleDao.GetByNameAsync("User", ct)
                ?? throw new InvalidOperationException("Role 'User' not found");
            user.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = defaultRole.Id
            });
            var otpCode = Random.Shared.Next(100000, 999999).ToString("D6");
            var otp = new OtpCode(user.Id, otpCode, 5);
            await _userDao.AddAsync(user, ct);
            await _userDao.AddOtpAsync(otp, ct);
            await _userDao.SaveChangesAsync(ct);
            await _emailService.SendOtpAsync(request.Email, request.FullName, otpCode, ct);
            return new RegisterResponse("Registration was successful A verification code has been sent to your email");
        }
    }
}