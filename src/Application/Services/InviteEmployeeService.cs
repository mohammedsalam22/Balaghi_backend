using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;

namespace Application.Services;

public sealed class InviteEmployeeService
{
    private readonly IUserDao _userDao;
    private readonly IRoleDao _roleDao;
    private readonly IGovernmentAgencyDao _agencyDao;
    private readonly IEmailService _emailService;

    public InviteEmployeeService(
        IUserDao userDao,
        IRoleDao roleDao,
        IGovernmentAgencyDao agencyDao,
        IEmailService emailService)
    {
        _userDao = userDao;
        _roleDao = roleDao;
        _agencyDao = agencyDao;
        _emailService = emailService;
    }
    public async Task<Guid> ExecuteAsync(Guid agencyId, string fullName, string email, CancellationToken ct = default)
{
    var agency = await _agencyDao.GetByIdAsync(agencyId)
        ?? throw new NotFoundException("The entity does not exist");
    var employeeRole = await _roleDao.GetByNameAsync("Employee", ct)
        ?? throw new InvalidOperationException("Role employee not found");
    if (await _userDao.GetByEmailAsync(email, ct) is not null)
        throw new InvalidOperationException("Email already taken");
    var user = new User(fullName, email, isEmailVerified: true);
    user.AssignToAgency(agencyId);
    user.UserRoles.Add(new UserRole { RoleId = employeeRole.Id });
    await _userDao.AddAsync(user, ct);
    await _userDao.SaveChangesAsync(ct);
    var code = new Random().Next(100000, 999999).ToString("D6");
var otp = new PasswordSetupOtp(user.Id, code);
    await _userDao.AddPasswordSetupOtpAsync(otp, ct);
    await _userDao.SaveChangesAsync(ct);
  var body = $" {code}";
    await _emailService.SendEmailAsync(email, "The code to actice account", body, ct);
    return user.Id;
}
    }
