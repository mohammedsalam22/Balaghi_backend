using Infrastructure.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Domain.Interfaces;

namespace Infrastructure.Repositories
{
    public class ComplaintRepository(AppDbContext context) : IComplaintRepository
    {
        public async Task AddAsync(Complaint complaint, CancellationToken ct = default)
        {
            await context.Complaints.AddAsync(complaint, ct);
        }

        public async Task<bool> ComplaintNumberExistsAsync(int complaintNumber, CancellationToken ct = default)
        {
            return await context.Complaints
                .AnyAsync(c => c.ComplaintNumber == complaintNumber, ct);
        }

        public async Task<Complaint?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await context.Complaints
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<List<Complaint>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await context.Complaints
                .Where(c => c.UserId == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<List<Complaint>> GetAllAsync(CancellationToken ct = default)
        {
            return await context.Complaints
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(ct);
        }

        public Task UpdateAsync(Complaint complaint, CancellationToken ct = default)
        {
            context.Complaints.Update(complaint);
            return Task.CompletedTask;
        }
    }
}

