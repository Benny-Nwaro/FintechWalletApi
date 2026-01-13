using System.Text;
using System.Text.Json;

namespace FintechWalletApi.Webhooks;

public class WebhookService : IWebhookService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public WebhookService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task SendFraudAlertAsync(FraudWebhookPayload payload)
    {
        var url = _config["Webhooks:FraudServiceUrl"];
        await PostAsync(url!, payload);
    }

    public async Task SendEmailAsync(EmailWebhookPayload payload)
    {
        var url = _config["Webhooks:EmailServiceUrl"];
        await PostAsync(url!, payload);
    }

    private async Task PostAsync(string url, object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Fire-and-forget style webhook
        await _httpClient.PostAsync(url, content);
    }
}
