namespace Application.DTOs
{
    public record CreateComplaintRequest(string Type, string AssignedPart, string Location, string Description);
}

