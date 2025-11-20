using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserDao
    {
        Task<User?> GetByIdWithRolesAsync(Guid userId, CancellationToken ct = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
        Task AddOtpAsync(OtpCode otp, CancellationToken ct = default);
        Task<User?> GetByEmailWithOtpsAsync(string email, CancellationToken ct = default);
        Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken ct = default);
        void MarkOtpAsUsed(OtpCode otp);
        Task SaveChangesAsync(CancellationToken ct = default);
        void UpdateUser(User user);
        void UpdateOtp(OtpCode otp);
        Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<PasswordSetupOtp?> GetValidSetupOtpAsync(string code, CancellationToken ct = default);
        Task AddPasswordSetupOtpAsync(PasswordSetupOtp otp, CancellationToken ct = default);

        
    }
}