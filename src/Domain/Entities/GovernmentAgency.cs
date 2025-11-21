using System.Collections.Generic;
using Domain.Common;

namespace Domain.Entities
{
    public class GovernmentAgency:BaseEntity
    {
        public string Name { get; set; } = null!;
         public ICollection<User>? Employees { get; set; } = new List<User>();
           public ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
    }
}
