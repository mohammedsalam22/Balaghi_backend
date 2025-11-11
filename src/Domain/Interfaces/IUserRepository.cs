using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdWithRolesAsync(Guid userId);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
        Task AddAsync(User user, CancellationToken ct = default);
        Task AddOtpAsync(OtpCode otp, CancellationToken ct = default);
        Task<User?> GetByEmailWithOtpsAsync(string email, CancellationToken ct);
        void MarkOtpAsUsed(OtpCode otp);
        Task<User?> GetByEmailWithRolesAsync(string email, CancellationToken ct = default);
    }
}
