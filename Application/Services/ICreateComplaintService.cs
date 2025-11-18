using Application.DTOs;

namespace Application.Services
{
    public interface ICreateComplaintService
    {
        Task<CreateComplaintResponse> ExecuteAsync(CreateComplaintRequest request, Guid userId, CancellationToken ct = default);
    }
}

