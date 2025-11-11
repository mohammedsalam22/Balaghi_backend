using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class RoleRepository(AppDbContext context) : IRoleRepository
    {
        public async Task<Role?> GetByNameAsync(string name, CancellationToken ct)
        => await context.Roles.FirstOrDefaultAsync(r => r.Name == name, ct);
    }
}
