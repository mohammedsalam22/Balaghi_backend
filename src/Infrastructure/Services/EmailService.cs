using Domain.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public sealed class EmailService : IEmailService
    {
        private readonly SmtpSettings _cfg;

        public EmailService(IOptions<SmtpSettings> options)
        {
            _cfg = options.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string body, CancellationToken ct = default)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_cfg.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_cfg.Host, _cfg.Port, SecureSocketOptions.StartTlsWhenAvailable, ct);
            await client.AuthenticateAsync(_cfg.Username, _cfg.Password, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }

        public async Task SendOtpAsync(string to, string fullName, string code, CancellationToken ct = default)
        {
            var html = $$"""
            <div style="font-family:Arial;text-align:center;padding:30px;background:#f8f9fa;border:1px solid #dee2e6;border-radius:10px;">
                <h2>Hello {{fullName}}!</h2>
                <p style="font-size:18px;">The code verificition is :</p>
                <h1 style="font-size:48px;letter-spacing:8px;color:#007bff;">{{code}}</h1>
                <p>The code is valid for a period of time <strong>5 minute</strong>just</p>
            </div>
            """;

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(_cfg.From));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = "The code to active account";
            message.Body = new TextPart("html") { Text = html };

            using var client = new SmtpClient();
            await client.ConnectAsync(_cfg.Host, _cfg.Port, SecureSocketOptions.StartTlsWhenAvailable, ct);
            await client.AuthenticateAsync(_cfg.Username, _cfg.Password, ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
