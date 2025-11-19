using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class RoleDto
    {
        public string Name { get; set; }
        public List<string> Permissions { get; set; }
    }

}
