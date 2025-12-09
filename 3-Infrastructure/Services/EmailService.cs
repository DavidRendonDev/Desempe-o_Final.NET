using System.Net.Mail;
using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string fullName)
    {
        var host = _config["Smtp:Host"]!;
        var port = int.Parse(_config["Smtp:Port"] ?? "587");
        var useSsl = bool.Parse(_config["Smtp:UseSsl"] ?? "false");
        var username = _config["Smtp:Username"]!;
        var password = _config["Smtp:Password"]!;
        var fromName = _config["Smtp:FromName"] ?? "TalentoPlus";
        var fromEmail = _config["Smtp:FromEmail"] ?? username;

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(fromName, fromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Welcome to TalentoPlus";
        message.Body = new TextPart("plain")
        {
            Text = $"Hello {fullName},\n\nYour registration was successful. You can authenticate when enabled.\n\nTalentoPlus S.A.S"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync(host, port, useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(username, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}