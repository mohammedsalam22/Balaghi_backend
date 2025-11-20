using Domain.Entities;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRoleDao
    {
        Task<Role?> GetByNameAsync(string name, CancellationToken ct = default);
    }
}