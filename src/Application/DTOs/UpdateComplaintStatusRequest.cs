using Domain.Entities;
namespace Application.DTOs;
public class UpdateComplaintStatusRequest
{
    public ComplaintStatus Status { get; set; }
}