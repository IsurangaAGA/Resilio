using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Resilio.Core.Interfaces;

namespace Resilio.Infrastructure.Services;

public sealed class GmailEmailSender : IEmailSender
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _senderName;
    private readonly string _senderEmail;
    private readonly string _appPassword;

    public GmailEmailSender(IConfiguration config)
    {
        _host = config["Email:SmtpHost"] ?? throw new InvalidOperationException("SMTP Host missing.");
        _port = int.Parse(config["Email:SmtpPort"] ?? "587");
        _senderName = config["Email:SenderName"] ?? throw new InvalidOperationException("SenderName missing.");
        _senderEmail = config["Email:SenderEmail"] ?? throw new InvalidOperationException("SenderEmail missing.");
        _appPassword = config["Email:AppPassword"] ?? throw new InvalidOperationException("AppPassword missing.");
    }

    public async Task SendOtpAsync(string toEmail, string otp, CancellationToken ct)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_senderName, _senderEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = "Your Resilio Verification Code";

        message.Body = new TextPart("html")
        {
            Text = $@"
                <div style='font-family: Arial;'>
                    <h2>Resilio Verification Code</h2>
                    <p>Your OTP code is:</p>
                    <h1 style='letter-spacing: 5px;'>{otp}</h1>
                    <p>This code expires in 5 minutes.</p>
                </div>"
        };

        using var client = new SmtpClient();

        try
        {
            await client.ConnectAsync(_host, _port, SecureSocketOptions.StartTls, ct);
            await client.AuthenticateAsync(_senderEmail, _appPassword, ct);
            await client.SendAsync(message, ct);
        }
        finally
        {
            await client.DisconnectAsync(true, ct);
        }
    }
}