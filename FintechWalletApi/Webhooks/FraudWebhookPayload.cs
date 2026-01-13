namespace FintechWalletApi.Webhooks;

public class FraudWebhookPayload
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = null!;
    public DateTime Timestamp { get; set; }
}
