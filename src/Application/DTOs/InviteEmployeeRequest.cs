namespace Application.DTOs;
public record InviteEmployeeRequest(
    Guid AgencyId,
    string FullName,
    string Email);