namespace Application.DTOs
{
public record GovernormentAgency<T>(string Message, bool Success = true, T? Data = default);
}