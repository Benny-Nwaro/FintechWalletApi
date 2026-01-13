namespace FintechWalletApi.Fraud;

public interface IFraudService
{
    void ValidateTransaction(Guid walletId, decimal amount);
}
