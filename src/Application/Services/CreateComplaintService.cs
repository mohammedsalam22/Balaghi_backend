using Application.DTOs;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class CreateComplaintService : ICreateComplaintService
    {
        private readonly IComplaintRepository _complaintRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateComplaintService(
            IComplaintRepository complaintRepository,
            IUnitOfWork unitOfWork)
        {
            _complaintRepository = complaintRepository;
            _unitOfWork = unitOfWork;
        }

        public virtual async Task<CreateComplaintResponse> ExecuteAsync(CreateComplaintRequest request, Guid userId, CancellationToken ct = default)
        {
            // Create the complaint entity
            var complaint = new Complaint(
                request.Type,
                request.AssignedPart,
                request.Location,
                request.Description,
                userId
            );

            // Ensure the complaint number is unique
            int maxAttempts = 100; // Prevent infinite loop
            int attempts = 0;
            while (await _complaintRepository.ComplaintNumberExistsAsync(complaint.ComplaintNumber, ct) && attempts < maxAttempts)
            {
                complaint.RegenerateComplaintNumber();
                attempts++;
            }

            if (attempts >= maxAttempts)
            {
                throw new InvalidOperationException("Unable to generate a unique complaint number. Please try again.");
            }

            // Add and save
            await _complaintRepository.AddAsync(complaint, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            return new CreateComplaintResponse(
                "Complaint created successfully",
                complaint.ComplaintNumber,
                complaint.Id
            );
        }
    }
}

