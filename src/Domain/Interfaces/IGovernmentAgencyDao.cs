using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IGovernmentAgencyDao
    {
        Task<GovernmentAgency?> GetByIdAsync(Guid id);
        Task<IEnumerable<GovernmentAgency>> GetAllAsync();
        Task AddAsync(GovernmentAgency agency);
        Task UpdateAsync(GovernmentAgency agency);
        Task DeleteAsync(Guid id);
    }
}
