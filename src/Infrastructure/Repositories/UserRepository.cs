using Infrastructure.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;
namespace Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;
        public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct)
        => await context.Users.AnyAsync(u => u.Email == email, ct);

        public async Task AddAsync(User user, CancellationToken ct)
            => await context.Users.AddAsync(user, ct);

        public async Task AddOtpAsync(OtpCode otp, CancellationToken ct)
            => await context.Set<OtpCode>().AddAsync(otp, ct);
        public async Task<User> GetByIdWithRolesAsync(Guid userId)
        {
            return await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
        public async Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken ct)
    => await _context.Users
        .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
        .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower(), ct);
        public async Task<User?> GetByEmailWithOtpsAsync(string email, CancellationToken ct)
    => await _context.Users
        .Include(u => u.OtpCodes)
        .FirstOrDefaultAsync(u => u.Email == email, ct);
        public void MarkOtpAsUsed(OtpCode otp)
            => otp.MarkAsUsed();
       
          
           
    }
}
