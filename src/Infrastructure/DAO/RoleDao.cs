using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataAccess
{
    public class RoleDao : IRoleDao
    {
        private readonly AppDbContext _context;

        public RoleDao(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> GetByNameAsync(string name, CancellationToken ct = default)
            => await _context.Roles.FirstOrDefaultAsync(r => r.Name == name, ct);
    }
}