using System.Security.Cryptography;
using System.Text;

namespace Resilio.API.Services;

public interface ITokenHasher
{
    string Hash(string token);
    bool Verify(string token, string expectedHash);
}

public sealed class TokenHasher : ITokenHasher
{
    private readonly byte[] _key;

    public TokenHasher(string secret)
    {
        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
            throw new ArgumentException("Token hashing secret must be at least 32 chars.", nameof(secret));

        _key = Encoding.UTF8.GetBytes(secret);
    }

    public string Hash(string token)
    {
        using var hmac = new HMACSHA256(_key);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hash);
    }

    public bool Verify(string token, string expectedHash)
    {
        var computed = Hash(token);
        return CryptographicOperations.FixedTimeEquals(
            Convert.FromBase64String(computed),
            Convert.FromBase64String(expectedHash)
        );
    }
}