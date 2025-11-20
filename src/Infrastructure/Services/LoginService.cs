using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace Infrastructure.Services
{
    public sealed class LoginService
    {
        private readonly IUserDao _userDao;
        private readonly IRefreshTokenDao _refreshTokenDao;
        private readonly IConfiguration _config;

        public LoginService(
            IUserDao userDao,
            IRefreshTokenDao refreshTokenDao,
            IConfiguration config)
        {
            _userDao = userDao;
            _refreshTokenDao = refreshTokenDao;
            _config = config;
        }

        public async Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken ct = default)
        {
            var user = await _userDao.GetByEmailWithRolesAsync(request.Email, ct)
                ?? throw new UnauthorizedAccessException("Incorrect email or password");

            if (!user.IsEmailVerified)
                throw new UnauthorizedAccessException("Email address not confirmed");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Incorrect email or password");
                var accessTokenExpiresAt = DateTime.UtcNow.AddHours(1);
            var accessToken = GenerateAccessToken(user);
            var refreshToken = Guid.NewGuid().ToString("N");
            var refreshEntity = new RefreshToken(user.Id, refreshToken, 7);
            await _refreshTokenDao.AddAsync(refreshEntity, ct);
            await _refreshTokenDao.SaveChangesAsync(ct); 
            
            var roles = user.UserRoles.Select(r => r.Role.Name).ToList();

            return new LoginResponse(
                accessToken,
                roles,
                refreshToken,
                accessTokenExpiresAt
            );

        }
        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FullName)
            };

            claims.AddRange(user.UserRoles.Select(ur => new Claim(ClaimTypes.Role, ur.Role.Name)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}