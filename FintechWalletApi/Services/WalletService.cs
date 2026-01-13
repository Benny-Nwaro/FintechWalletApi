using FintechWalletApi.Data;
using FintechWalletApi.Exceptions;
using FintechWalletApi.Models;
using FintechWalletApi.Fraud;
using FintechWalletApi.Webhooks;
using FintechWalletApi.Audit;
namespace FintechWalletApi.Services;

public class WalletService : IWalletService
{
    private readonly AppDbContext _context;
    private readonly IFraudService _fraudService;
    private readonly IWebhookService _webhookService;
    private readonly IAuditService _auditService;


    public WalletService(AppDbContext context, IFraudService fraudService, IWebhookService webhookService, IAuditService auditService)
    {
        _context = context;
        _fraudService = fraudService;
        _webhookService = webhookService;
        _auditService = auditService;
    }

    public Wallet CreateWallet(Guid userId)
    {
        var wallet = new Wallet
        {
            UserId = userId,
            Balance = 0
        };

        _context.Wallets.Add(wallet);
        _context.SaveChanges();

        return wallet;
    }

    public Wallet AddMoney(Guid walletId, decimal amount)
    {
        _fraudService.ValidateTransaction(walletId, amount);
        if (amount <= 0)
            throw new BadRequestException("Amount must be greater than zero");

        var wallet = _context.Wallets.Find(walletId)
            ?? throw new NotFoundException("Wallet not found");

        wallet.Balance += amount;

        _context.Transactions.Add(new Transaction
        {
            WalletId = wallet.Id,
            Amount = amount,
            Type = "Credit",
            Description = "Wallet top-up"
        });

        _context.SaveChanges();
        return wallet;
    }

    public Wallet WithdrawMoney(Guid walletId, decimal amount)
    {
        _fraudService.ValidateTransaction(walletId, amount);
        if (amount <= 0)
            throw new BadRequestException("Amount must be greater than zero");

        var wallet = _context.Wallets.Find(walletId)
            ?? throw new NotFoundException("Wallet not found");

        if (wallet.Balance < amount)
            throw new ForbiddenException("Insufficient funds");

        wallet.Balance -= amount;

        _context.Transactions.Add(new Transaction
        {
            WalletId = wallet.Id,
            Amount = -amount,
            Type = "Debit",
            Description = "Wallet withdrawal"
        });

        _context.SaveChanges();
        return wallet;
    }

    public async Task<Wallet> Transfer(Guid fromWalletId, Guid toWalletId, decimal amount)
    {
        _fraudService.ValidateTransaction(fromWalletId, amount);
        if (amount <= 0)
            throw new BadRequestException("Transfer amount must be greater than zero");

        if (fromWalletId == toWalletId)
            throw new BadRequestException("Cannot transfer to the same wallet");

        using var dbTransaction = _context.Database.BeginTransaction();

        try
        {
            var senderWallet = _context.Wallets
                .FirstOrDefault(w => w.Id == fromWalletId)
                ?? throw new NotFoundException("Sender wallet not found");

            var receiverWallet = _context.Wallets
                .FirstOrDefault(w => w.Id == toWalletId)
                ?? throw new NotFoundException("Receiver wallet not found");

            if (senderWallet.Balance < amount)
                throw new ForbiddenException("Insufficient funds");

            // Debit sender
            senderWallet.Balance -= amount;
            _context.Transactions.Add(new Transaction
            {
                WalletId = senderWallet.Id,
                Amount = -amount,
                Type = "Transfer",
                Description = $"Transfer to wallet {receiverWallet.Id}"
            });

            // Credit receiver
            receiverWallet.Balance += amount;
            _context.Transactions.Add(new Transaction
            {
                WalletId = receiverWallet.Id,
                Amount = amount,
                Type = "Transfer",
                Description = $"Transfer from wallet {senderWallet.Id}"
            });

            _context.SaveChanges();
            dbTransaction.Commit();

            await _auditService.LogAsync(
                null,
                "Transfer",
                "Wallet",
                "Success",
                new
                {
                    FromWallet = fromWalletId,
                    ToWallet = toWalletId,
                    Amount = amount
                });


            _ = _webhookService.SendEmailAsync(new EmailWebhookPayload
                {
                    Email = "user@example.com",
                    Subject = "Transfer Completed",
                    Message = $"You sent {amount} successfully."
                });


            return senderWallet;
        }
        catch
        {
            dbTransaction.Rollback();
            await _auditService.LogAsync(
                null,
                "Transfer",
                "Wallet",
                "Failure",
                new
                {
                    FromWallet = fromWalletId,
                    ToWallet = toWalletId,
                    Amount = amount,
                    Reason = "Transfer failed"
                });

            throw;
        }
    }

    public async Task<decimal> GetBalance(Guid walletId)
    {
        var wallet = _context.Wallets.Find(walletId)
            ?? throw new NotFoundException("Wallet not found");

        return wallet.Balance;
    }


}
