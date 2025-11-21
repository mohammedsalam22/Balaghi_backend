using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration; 

namespace Infrastructure.Persistence
{
    public static class DbInitializer
    {
        private const string AdminEmail = "admin@system.gov.sa";
        private const string AdminFullName = "Admin";
        private const string AdminPassword = "Admin@123456";

        private const string TestUserEmail = "user@test.com";
        private const string TestUserFullName ="User";
        private const string TestUserPassword = "User@123456";

        public static async Task SeedAsync(
            AppDbContext context,
            IPasswordHasher passwordHasher,
            IConfiguration configuration)
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
                Console.WriteLine(": Admin, Employee, User");
            }
            if (!await context.Users.AnyAsync(u => u.Email == AdminEmail))
            {
                var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");

                var adminUser = CreateUser(AdminFullName, AdminEmail, AdminPassword, passwordHasher);
                adminUser.UserRoles.Add(new UserRole { RoleId = adminRole.Id });

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();

                PrintUserInfo(adminUser, "Admin", configuration);
            }
            if (!await context.Users.AnyAsync(u => u.Email == TestUserEmail))
            {
                var userRole = await context.Roles.FirstAsync(r => r.Name == "User");

                var testUser = CreateUser(TestUserFullName, TestUserEmail, TestUserPassword, passwordHasher);
                testUser.UserRoles.Add(new UserRole { RoleId = userRole.Id });

                await context.Users.AddAsync(testUser);
                await context.SaveChangesAsync();

                PrintUserInfo(testUser, "User", configuration);
            }
        }

        private static User CreateUser(string fullName, string email, string plainPassword, IPasswordHasher hasher)
        {
            var hash = hasher.Hash(plainPassword);
            return new User(fullName, email, hash)
            {
                Id = Guid.NewGuid(),
                IsEmailVerified = true
            };
        }

        private static void PrintUserInfo(User user, string role, IConfiguration config)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"Name       : {user.FullName}");
            Console.WriteLine($"Email    : {user.Email}");
            Console.WriteLine($" Password  : {(role == "Admin" ? AdminPassword : TestUserPassword)}");
            Console.WriteLine($"Role       : {role}");
            var token = GenerateJwtToken(user, role, config);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(token);
            Console.ResetColor();
        }
        private static string GenerateJwtToken(User user, string role, IConfiguration config)
        {
            var key = config["Jwt:Key"] 
                ?? "your-very-strong-secret-key-must-be-32-chars+!!";

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}