using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
public class ComplaintDao : IComplaintDao
{
    private readonly AppDbContext _context;

    public ComplaintDao(AppDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(Complaint complaint, CancellationToken ct = default)
    {
        await _context.Complaints.AddAsync(complaint, ct);
    }
public async Task<Complaint?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default)
{
    return await _context.Complaints
        .Include(c => c.Agency)
        .Include(c => c.Attachments)       
        .FirstOrDefaultAsync(c => c.TrackingNumber == trackingNumber, ct);
}
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }
public async Task<List<Complaint>> GetByAgencyIdAsync(Guid agencyId, CancellationToken ct = default)
{
    return await _context.Complaints
        .Include(c => c.Citizen)
        .Include(c => c.Attachments)
        .Where(c => c.AgencyId == agencyId)
        .OrderByDescending(c => c.CreatedAt)
        .ToListAsync(ct);
}
public async Task<Complaint?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default)
{
    return await _context.Complaints
        .Include(c => c.Citizen)
        .Include(c => c.Agency)
        .Include(c => c.Attachments)
        .FirstOrDefaultAsync(c => c.Id == id, ct);
}
}