using Domain.Entities;
public interface IComplaintDao
{
    Task AddAsync(Complaint complaint, CancellationToken ct = default);
    Task<Complaint?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct = default);
    Task<Complaint?> GetByIdWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task<List<Complaint>> GetByAgencyIdAsync(Guid agencyId, CancellationToken ct = default);
}