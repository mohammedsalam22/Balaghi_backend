using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess
{
    public class UserDao : IUserDao
    {
        private readonly AppDbContext _context;

        public UserDao(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);

        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
            => await _context.Users.AnyAsync(u => u.Email == email, ct);

        public async Task AddAsync(User user, CancellationToken ct = default)
            => await _context.Users.AddAsync(user, ct);

        public async Task AddOtpAsync(OtpCode otp, CancellationToken ct = default)
            => await _context.OtpCodes.AddAsync(otp, ct);

        public async Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken ct = default)
            => await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId, ct);

        public async Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken ct = default)
            => await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), ct);

        public async Task<User?> GetByEmailWithOtpsAsync(string email, CancellationToken ct = default)
            => await _context.Users
                .Include(u => u.OtpCodes)
                .FirstOrDefaultAsync(u => u.Email == email, ct);
        public void MarkOtpAsUsed(OtpCode otp)
        {
            otp.IsUsed = true;
        }

        public void MarkUserEmailVerified(User user)
        {
            user.IsEmailVerified = true;
        }
        public void UpdateOtp(OtpCode otp) => _context.OtpCodes.Update(otp);
public void UpdateUser(User user) => _context.Users.Update(user);
    }
}
