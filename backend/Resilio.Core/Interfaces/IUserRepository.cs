using Resilio.Core.Models;

namespace Resilio.Core.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByPhoneOrEmailAsync(string identifier, CancellationToken ct);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken ct);
    Task<User> CreateAsync(User user, CancellationToken ct);
}