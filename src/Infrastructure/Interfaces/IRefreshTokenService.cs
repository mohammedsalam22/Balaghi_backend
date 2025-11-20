using Application.DTOs;
namespace Application.UseCases.Auth;

public interface IRefreshTokenService
{
    Task<LoginResponse> ExecuteAsync(string oldRefreshToken, CancellationToken ct = default);
}