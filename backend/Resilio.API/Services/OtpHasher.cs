using System.Security.Cryptography;
using System.Text;

namespace Resilio.API.Services;

public interface IOtpHasher
{
    string Hash(string identifier, string otp);
    bool Verify(string identifier, string otp, string expectedHash);
}

public sealed class OtpHasher : IOtpHasher
{
    private readonly byte[] _key;

    public OtpHasher(string hmacSecret)
    {
        if (string.IsNullOrWhiteSpace(hmacSecret) || hmacSecret.Length < 32)
            throw new ArgumentException("OTP HMAC secret must be at least 32 chars.", nameof(hmacSecret));

        _key = Encoding.UTF8.GetBytes(hmacSecret);
    }

    public string Hash(string identifier, string otp)
    {
        // Binding hash to identifier reduces replay attacks across users
        var payload = $"{identifier}|{otp}";
        using var hmac = new HMACSHA256(_key);
        var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToBase64String(bytes);
    }

    public bool Verify(string identifier, string otp, string expectedHash)
    {
        var computed = Hash(identifier, otp);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(computed),
            Convert.FromBase64String(expectedHash)
        );
    }
}