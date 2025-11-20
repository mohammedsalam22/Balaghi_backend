// Infrastructure/Persistence/DbInitializer.cs
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public static class DbInitializer
    {
        private const string AdminEmail = "admin@system.gov.sa";
        private const string AdminFullName = "Admin";
        private const string AdminPassword = "Admin@123456";
        public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
        {
            if (!await context.Roles.AnyAsync())
            {
                var roles = new[]
                {
                    new Role { Id = Guid.NewGuid(), Name = "Admin" },
                    new Role { Id = Guid.NewGuid(), Name = "Employee" },
                    new Role { Id = Guid.NewGuid(), Name = "User" }
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();

                Console.WriteLine(" Admin, Employee, Use");
            }
            if (!await context.Users.AnyAsync(u => u.Email == AdminEmail))
            {
                var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin")
                    ?? throw new Exception("Role Admin Not found");

                var passwordHash = passwordHasher.Hash(AdminPassword);

                var adminUser = new User(AdminFullName, AdminEmail, passwordHash)
                {
                    Id = Guid.NewGuid(),
                    IsEmailVerified = true 
                };
                adminUser.UserRoles.Add(new UserRole
                {
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                });

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Name : {AdminFullName}");
                Console.WriteLine($"email: {AdminEmail}");
                Console.WriteLine($"password : {AdminPassword}");
                Console.ResetColor();
            }
        }
    }
}