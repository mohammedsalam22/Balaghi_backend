using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Exceptions
{
    public class OtpInvalidException : Exception
    {
        public OtpInvalidException() : base("The verification code is incorrect") { }
    }
}
