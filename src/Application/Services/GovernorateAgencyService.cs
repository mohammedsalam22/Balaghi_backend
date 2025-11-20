using Domain.Entities;
using Domain.Interfaces;
using Application.DTOs;
using Domain.Exceptions;
namespace Application.Services
{
    public sealed class  GovernorateAgencyService
    {
        private  IGovernmentAgencyDao _dao;

        public GovernorateAgencyService(IGovernmentAgencyDao dao)
        {
            _dao = dao;
        }

        public async Task<IEnumerable<GovernmentAgencyDto>> GetAllAsync()
        {
            var agencies = await _dao.GetAllAsync();
            return agencies.Select(a => new GovernmentAgencyDto(
                a.Id,
                a.Name,
                a.Employees?.Select(e => new EmployeeDto(e.Id, e.FullName, e.Email)) ?? new List<EmployeeDto>()
            ));
        }

        public async Task<GovernmentAgencyDto> GetByIdAsync(Guid id)
        {
            var agency = await _dao.GetByIdAsync(id);
            if (agency == null) throw new NotFoundException("Agency not found");
            return new GovernmentAgencyDto(
                agency.Id,
                agency.Name,
                agency.Employees?.Select(e => new EmployeeDto(e.Id, e.FullName, e.Email)) ?? new List<EmployeeDto>()
            );
        }

        public async Task<GovernmentAgencyDto> CreateAsync(string name)
        {
            var agency = new GovernmentAgency
            {
                Name = name,
                Employees = null
            };
            await _dao.AddAsync(agency);
            return new GovernmentAgencyDto(agency.Id, agency.Name, new List<EmployeeDto>());
        }

        public async Task UpdateAsync(Guid id, string name)
        {
            var agency = await _dao.GetByIdAsync(id);
            if (agency == null) throw new NotFoundException("Agency not found");
            agency.Name = name;
            await _dao.UpdateAsync(agency);
        }

        public async Task DeleteAsync(Guid id)
        {
            var agency = await _dao.GetByIdAsync(id);
            if (agency == null) throw new NotFoundException("Agency not found");
            await _dao.DeleteAsync(id);
        }
    }
}
