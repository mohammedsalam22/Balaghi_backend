using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.Roles.AnyAsync())
            {
                var user = new Role { Id = Guid.NewGuid(), Name = "User" };
                var employee = new Role { Id = Guid.NewGuid(), Name = "Empolyee" };
                var admin = new Role { Id = Guid.NewGuid(), Name = "Admin" };
                var permissions = new List<Permission>
                {
                    new() { Id = Guid.NewGuid(), Name = "view" },
                    new() { Id = Guid.NewGuid(), Name = "read" },
                    new() { Id = Guid.NewGuid(), Name = "write" }
                };
                var rolePermissions = new List<RolePermission>
                {
                    new() { Role = user, Permission = permissions[0] },
                    new() { Role = employee, Permission = permissions[1] },
                    new() { Role = admin, Permission = permissions[1] },
                    new() { Role = admin, Permission = permissions[2] }
                };
               await context.Permissions.AddRangeAsync(permissions);
                await context.Roles.AddRangeAsync(new[] { user, employee, admin});
                await context.RolePermissions.AddRangeAsync(rolePermissions);
                await context.SaveChangesAsync();
            }
        }
    }
}
