using System.Text.Json;
using FintechWalletApi.Data;

namespace FintechWalletApi.Audit;

public class AuditService : IAuditService
{
    private readonly AppDbContext _context;

    public AuditService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(
        Guid? userId,
        string action,
        string resource,
        string status,
        object? metadata = null)
    {
        var log = new AuditLog
        {
            UserId = userId,
            Action = action,
            Resource = resource,
            Status = status,
            Metadata = metadata == null
                ? null
                : JsonSerializer.Serialize(metadata)
        };

        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync();
    }
}
