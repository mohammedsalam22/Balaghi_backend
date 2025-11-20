namespace Application.DTOs
{
    public record GovernmentAgencyDto(
        Guid Id,
        string Name,
        IEnumerable<EmployeeDto> Employees
    );

    public record EmployeeDto(Guid Id, string UserName, string Email);
}
