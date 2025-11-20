using Application.DTOs;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services
{
    public sealed class VerifyOtpService
    {
        private readonly IUserDao _userDao;

        public VerifyOtpService(IUserDao userDao)
        {
            _userDao = userDao;
        }

        public async Task<VerifyOtpResponse> ExecuteAsync(VerifyOtpRequest request, CancellationToken ct = default)
        {
            var user = await _userDao.GetByEmailWithOtpsAsync(request.Email, ct)
                ?? throw new UserNotFoundException(request.Email);
            if (user.IsEmailVerified)
                return new VerifyOtpResponse("The email address is already verified");
            var latestOtp = user.OtpCodes
                .Where(o => !o.IsUsed)
                .OrderByDescending(o => o.CreatedAt)
                .FirstOrDefault();

            if (latestOtp is null || !latestOtp.IsValid(request.Code))
                throw new OtpInvalidException();
            user.IsEmailVerified = true;      
            latestOtp.IsUsed = true;     
            _userDao.UpdateUser(user);
            _userDao.UpdateOtp(latestOtp);
            await _userDao.SaveChangesAsync(ct);
            return new VerifyOtpResponse("Your email has been successfully verified You can now log in");
        }
    }
}