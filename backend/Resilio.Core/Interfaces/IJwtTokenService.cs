namespace Resilio.Core.Interfaces;

public interface IJwtTokenService
{
    (string Token, int ExpiresInSeconds) CreateAccessToken(Guid userId, string role, int tier);
}