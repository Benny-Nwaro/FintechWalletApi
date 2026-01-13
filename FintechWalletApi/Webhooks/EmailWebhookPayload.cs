namespace FintechWalletApi.Webhooks;

public class EmailWebhookPayload
{
    public string Email { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
}
