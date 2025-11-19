using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Interfaces;

namespace Application.Services
{
    public class AuthorizationService :IPermissionService
    {
        private readonly IUserRepository _userRepository;
        public AuthorizationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<bool> HasPermissionAsync(Guid userId, string permissionName)
        {
            var user=await _userRepository.GetByIdWithRolesAsync(userId);
            return user.UserRoles.SelectMany(ur => ur.Role.RolePermissions)
            .Any(rp => rp.Permission.Name == permissionName);
        }
    }
}
