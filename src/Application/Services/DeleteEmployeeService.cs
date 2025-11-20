namespace Application.Services
{
    using Domain.Interfaces;
    using Domain.Exceptions;
    using Domain.Entities;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class DeleteEmployeeService
    {
        private readonly IUserDao _userDao;
        public DeleteEmployeeService(IUserDao userDao)
        {
            _userDao = userDao;
        }
        public async Task DeleteEmployeeAsync(Guid userId, CancellationToken ct = default)
        {
            var user = await _userDao.GetByIdAsync(userId, ct);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            _userDao.Remove(user);
            await _userDao.SaveChangesAsync(ct);
        }
    }
}