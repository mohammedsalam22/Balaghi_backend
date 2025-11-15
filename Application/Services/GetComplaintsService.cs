using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;
using System.Linq;

namespace Application.Services
{
    public class GetComplaintsService
    {
        private readonly IComplaintRepository _complaintRepository;

        public GetComplaintsService(IComplaintRepository complaintRepository)
        {
            _complaintRepository = complaintRepository;
        }

        public async Task<GetComplaintsResponse> ExecuteAsync(Guid userId, bool isAdmin, CancellationToken ct = default)
        {
            List<Complaint> complaints;

            if (isAdmin)
            {
                // Admin can see all complaints
                complaints = await _complaintRepository.GetAllAsync(ct);
            }
            else
            {
                // Regular users can only see their own complaints
                complaints = await _complaintRepository.GetByUserIdAsync(userId, ct);
            }

            var complaintDtos = complaints.Select(c => new ComplaintDto(
                c.Id,
                c.Type,
                c.AssignedPart,
                c.Location,
                c.Description,
                c.ComplaintNumber,
                c.Status,
                c.UserId,
                c.CreatedAt
            )).ToList();

            return new GetComplaintsResponse(complaintDtos);
        }
    }
}

