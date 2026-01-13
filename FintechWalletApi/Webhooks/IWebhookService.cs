namespace FintechWalletApi.Webhooks;

public interface IWebhookService
{
    Task SendFraudAlertAsync(FraudWebhookPayload payload);
    Task SendEmailAsync(EmailWebhookPayload payload);
}
