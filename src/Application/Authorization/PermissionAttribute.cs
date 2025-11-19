using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Authorization
{
    public class PermissionAttribute : Attribute
    {
        public string Name { get; }

        public PermissionAttribute(string name)
        {
            Name = name;
        }
    }
}
