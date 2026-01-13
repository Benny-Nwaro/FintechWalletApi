using FintechWalletApi.Models;

namespace FintechWalletApi.Services;

public interface IWalletService
{
    Wallet CreateWallet(Guid userId);
    Wallet AddMoney(Guid walletId, decimal amount);
    Wallet WithdrawMoney(Guid walletId, decimal amount);
    Task<Wallet> Transfer(Guid fromWalletId, Guid toWalletId, decimal amount);
    Task<decimal> GetBalance(Guid walletId);

}
