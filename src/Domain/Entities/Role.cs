using Domain.Common;

namespace Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; } = null!;
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public Guid? GovernmentEntityId { get; private set; }
public GovernmentEntity? GovernmentEntity { get; private set; }
    }
}