using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
namespace Infrastructure.Services
{
    public sealed class LoginService
    {
        private readonly IUserRepository _userRepo;
        private readonly IRefreshTokenRepository _refreshRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public LoginService(
            IUserRepository userRepo,
            IRefreshTokenRepository refreshRepo,
            IUnitOfWork unitOfWork,
            IConfiguration config)
        {
            _userRepo = userRepo;
            _refreshRepo = refreshRepo;
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<LoginResponse> ExecuteAsync(LoginRequest request, CancellationToken ct)
        {
            var user = await _userRepo.GetByEmailWithRolesAsync(request.Email, ct)
                ?? throw new UnauthorizedAccessException("Incorrect email or password");

            if (!user.IsEmailVerified)
                throw new UnauthorizedAccessException("Email address not confirmed");

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Incorrect email or password");

            var accessToken = GenerateAccessToken(user);
            var refreshToken = Guid.NewGuid().ToString();

            var refreshEntity = new RefreshToken(user.Id, refreshToken, 7);
            await _refreshRepo.AddAsync(refreshEntity, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return new LoginResponse(accessToken, refreshToken, DateTime.UtcNow.AddHours(1));
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