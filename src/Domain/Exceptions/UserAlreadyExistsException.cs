using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class UserAlreadyExistsException(string email)
    : Exception($"  this {email} arleady taken ")
    { }
}
