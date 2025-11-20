using Domain.Interfaces;

namespace Application.Services
{
    public class AuthorizationService : IPermissionService
    {
        private readonly IUserDao _userDao;
        public AuthorizationService(IUserDao userDao) => _userDao = userDao;

        public async Task<bool> HasPermissionAsync(Guid userId, string permissionName)
        {
            var user = await _userDao.GetByIdWithRolesAsync(userId);
            return user?.UserRoles
                .SelectMany(ur => ur.Role.RolePermissions)
                .Any(rp => rp.Permission.Name == permissionName) 
                ?? false;
        }
    }
}