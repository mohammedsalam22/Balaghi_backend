using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class UpdateComplaintStatusService
    {
        private readonly IComplaintRepository _complaintRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateComplaintStatusService(
            IComplaintRepository complaintRepository,
            IUnitOfWork unitOfWork)
        {
            _complaintRepository = complaintRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<UpdateComplaintStatusResponse> ExecuteAsync(
            Guid complaintId,
            ComplaintStatus newStatus,
            CancellationToken ct = default)
        {
            var complaint = await _complaintRepository.GetByIdAsync(complaintId, ct);
            
            if (complaint == null)
            {
                throw new KeyNotFoundException("Complaint not found");
            }

            complaint.Status = newStatus;
            await _complaintRepository.UpdateAsync(complaint, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return new UpdateComplaintStatusResponse("Complaint status updated successfully");
        }
    }
}

