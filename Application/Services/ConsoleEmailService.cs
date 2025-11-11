using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public sealed class ConsoleEmailService : IEmailService
    {
        public Task SendOtpAsync(string to, string fullName, string code, CancellationToken ct = default)
        {
            Console.WriteLine($"[OTP] to: {to} | Name: {fullName} | code : {code}");
            return Task.CompletedTask;
        }

        public Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            Console.WriteLine($"[EMAIL] to: {to}");
            Console.WriteLine($"subject: {subject}");
            Console.WriteLine($"message:\n{body}");
            Console.WriteLine("----------------------------------------");
            return Task.CompletedTask;
        }
    }
}