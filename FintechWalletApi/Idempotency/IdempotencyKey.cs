namespace FintechWalletApi.Idempotency;

public class IdempotencyKey
{
    public Guid Id { get; set; }
    public string Key { get; set; } = null!;
    public string RequestPath { get; set; } = null!;
    public string RequestBodyHash { get; set; } = null!;
    public string Response { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
