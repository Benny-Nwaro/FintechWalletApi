namespace FintechWalletApi.Audit;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }      // Nullable for system actions
    public string Action { get; set; } = null!;
    public string Resource { get; set; } = null!;
    public string Status { get; set; } = null!;  // Success / Failure
    public string? Metadata { get; set; }        // JSON string
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
