using Domain.Entities;
namespace Domain.Interfaces
{
    public interface IGovernmentEntityDao
    {
        Task<IReadOnlyList<GovernmentEntity>> GetAllWithEmployeesAsync(CancellationToken ct = default);
        Task<GovernmentEntity?> GetByIdWithEmployeesAsync(Guid id, CancellationToken ct = default);
    }
}