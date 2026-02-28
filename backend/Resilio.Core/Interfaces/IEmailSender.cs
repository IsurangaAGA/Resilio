namespace Resilio.Core.Interfaces;

public interface IEmailSender
{
    Task SendOtpAsync(string toEmail, string otp, CancellationToken ct);
}