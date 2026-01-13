using FintechWalletApi.Data;
using FintechWalletApi.Exceptions;
using Microsoft.EntityFrameworkCore;
using FintechWalletApi.Webhooks;
using FintechWalletApi.Audit;
namespace FintechWalletApi.Fraud;


public class FraudService : IFraudService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly IWebhookService _webhookService;
    private readonly IAuditService _auditService;

    public FraudService(AppDbContext context, IConfiguration config, IWebhookService webhookService, IAuditService auditService)
    {
        _context = context;
        _config = config;
        _auditService = auditService;
        _webhookService = webhookService;
    }

    public async void ValidateTransaction(Guid walletId, decimal amount)
    {
        var maxAmount = decimal.Parse(_config["FraudRules:MaxTransactionAmount"]!);
        var dailyLimit = decimal.Parse(_config["FraudRules:DailyLimit"]!);
        var maxTxPerDay = int.Parse(_config["FraudRules:MaxTransactionsPerDay"]!);

        // Rule 1: Per-transaction limit
        if (amount > maxAmount)
            {
                _ = _webhookService.SendFraudAlertAsync(new FraudWebhookPayload
                {
                    WalletId = walletId,
                    Amount = amount,
                    Reason = "Transaction amount exceeded limit",
                    Timestamp = DateTime.UtcNow
                });
                await _auditService.LogAsync(
                    null,
                    "FraudRuleTriggered",
                    "Transaction",
                    "Failure",
                    new { walletId, amount });

                throw new BadRequestException("Transaction amount exceeds allowed limit");
            }

        var today = DateTime.UtcNow.Date;

        var todaysTransactions = _context.Transactions
            .Where(t => t.WalletId == walletId && t.CreatedAt >= today)
            .ToList();

        // Rule 2: Daily transaction count (velocity check)
        if (todaysTransactions.Count >= maxTxPerDay)
            throw new BadRequestException("Too many transactions today");

        // Rule 3: Daily amount limit
        var dailyTotal = todaysTransactions.Sum(t => Math.Abs(t.Amount));
        if (dailyTotal + amount > dailyLimit){
            await _auditService.LogAsync(
                null,
                "FraudRuleTriggered",
                "Transaction",
                "Failure",
                new { walletId, amount });


            throw new BadRequestException("Daily transaction limit exceeded");
        }
    }
}
