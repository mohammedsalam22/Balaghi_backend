using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class GovernmentAgency:BaseEntity
    {
        public string Name { get; set; } = null!;
         public ICollection<User>? Employees { get; set; } = new List<User>();
    }
}
