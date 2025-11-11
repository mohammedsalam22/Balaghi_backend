using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IEmailService
    {
        Task SendOtpAsync(string to, string fullName, string code, CancellationToken ct = default);
        Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default);
    }

}
