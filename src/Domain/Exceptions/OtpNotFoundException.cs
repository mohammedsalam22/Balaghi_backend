using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class OtpNotFoundException : Exception
    {
        public OtpNotFoundException() : base("The verification code is not found or has expired") { }
    }
}
