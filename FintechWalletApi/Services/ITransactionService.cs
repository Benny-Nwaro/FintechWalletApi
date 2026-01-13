using FintechWalletApi.Common;
using FintechWalletApi.DTOs;

namespace FintechWalletApi.Services;

public interface ITransactionService
{
    PagedResult<TransactionDto> GetWalletTransactions(Guid walletId, TransactionQueryParams query);
}
