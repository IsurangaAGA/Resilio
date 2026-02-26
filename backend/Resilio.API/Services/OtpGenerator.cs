using System.Security.Cryptography;

namespace Resilio.API.Services;

public interface IOtpGenerator
{
    string Generate6Digits();
}

public sealed class OtpGenerator : IOtpGenerator
{
    public string Generate6Digits()
    {
        // cryptographically secure random 000000-999999
        var value = RandomNumberGenerator.GetInt32(0, 1_000_000);
        return value.ToString("D6");
    }
}