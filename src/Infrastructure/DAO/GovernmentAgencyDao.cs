using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class GovernmentAgencyDao : IGovernmentAgencyDao
    {
        private readonly AppDbContext _context;

        public GovernmentAgencyDao(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(GovernmentAgency agency)
        {
            _context.GovernmentAgencies.Add(agency);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var agency = await _context.GovernmentAgencies.FindAsync(id);
            if (agency != null)
            {
                _context.GovernmentAgencies.Remove(agency);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<GovernmentAgency>> GetAllAsync()
        {
            return await _context.GovernmentAgencies
                .Include(a => a.Employees) 
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<GovernmentAgency?> GetByIdAsync(Guid id)
        {
            return await _context.GovernmentAgencies
                .Include(a => a.Employees)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(GovernmentAgency agency)
        {
            _context.GovernmentAgencies.Update(agency);
            await _context.SaveChangesAsync();
        }
    }
}
