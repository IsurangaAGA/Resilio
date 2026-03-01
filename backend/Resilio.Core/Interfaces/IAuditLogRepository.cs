namespace Resilio.Core.Interfaces;

public interface IAuditLogRepository
{
    Task WriteAsync(Guid? userId, string action, string? metadataJson, string? ip, string? userAgent, CancellationToken ct);
}