using Domain.Common;

namespace Domain.Entities
{
    public class GovernmentEntity : BaseEntity
    {
        public string Name { get; private set; } = null!;
        public ICollection<User> Employees { get; set; } = new List<User>();
        public ICollection<Role> DefaultRoles { get; set; } = new List<Role>();
        private GovernmentEntity() { }
        public GovernmentEntity( string name)
        {
            Name = name;
        }
    }
}