using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IComplaintRepository
    {
        Task AddAsync(Complaint complaint, CancellationToken ct = default);
        Task<bool> ComplaintNumberExistsAsync(int complaintNumber, CancellationToken ct = default);
        Task<Complaint?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<Complaint>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<List<Complaint>> GetAllAsync(CancellationToken ct = default);
        Task UpdateAsync(Complaint complaint, CancellationToken ct = default);
    }
}

