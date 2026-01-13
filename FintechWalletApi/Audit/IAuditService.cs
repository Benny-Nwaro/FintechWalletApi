namespace FintechWalletApi.Audit;

public interface IAuditService
{
    Task LogAsync(
        Guid? userId,
        string action,
        string resource,
        string status,
        object? metadata = null);
}
