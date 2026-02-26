using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Resilio.Core.Interfaces;

namespace Resilio.API.Services;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expiryMinutes;
    private readonly SymmetricSecurityKey _key;

    public JwtTokenService(IConfiguration config)
    {
        _issuer = config["Auth:JwtIssuer"] ?? "Resilio";
        _audience = config["Auth:JwtAudience"] ?? "ResilioClient";
        _expiryMinutes = int.TryParse(config["Auth:JwtExpiryMinutes"], out var m) ? m : 60;

        var keyString = config["Auth:JwtKey"] ?? throw new InvalidOperationException("Auth:JwtKey missing.");
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
    }

    public (string Token, int ExpiresInSeconds) CreateAccessToken(Guid userId, string role, int tier)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(_expiryMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(ClaimTypes.Role, role),
            new("tier", tier.ToString())
        };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(_issuer, _audience, claims, now, expires, creds);

        return (new JwtSecurityTokenHandler().WriteToken(token), (int)(expires - now).TotalSeconds);
    }
}