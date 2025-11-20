using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IPasswordResetDao
    {
        Task AddAsync(PasswordResetToken token, CancellationToken ct = default);
        Task<PasswordResetToken?> GetByTokenHashAsync(string tokenHash, CancellationToken ct = default);
        Task<PasswordResetToken?> GetByTokenAsync(string token, CancellationToken ct = default);
        void MarkAsUsed(PasswordResetToken token);
    }
}