
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public sealed class CompleteEmployeeSetupService
{
    private readonly IUserDao _userDao;
    private readonly IPasswordHasher _hasher;

    public CompleteEmployeeSetupService(IUserDao userDao, IPasswordHasher hasher)
    {
        _userDao = userDao;
        _hasher = hasher;
    }

    public async Task CompleteAsync(string code, string newPassword, CancellationToken ct = default)
    {
        var otp = await _userDao.GetValidSetupOtpAsync(code, ct)
            ?? throw new UnauthorizedAccessException("The code is invalid or expired");

        if (otp.User.HasPassword())
            throw new InvalidOperationException("The password has already been set");

        otp.User.SetPassword(_hasher.Hash(newPassword));
        otp.MarkAsUsed();

        await _userDao.SaveChangesAsync(ct);
    }
}