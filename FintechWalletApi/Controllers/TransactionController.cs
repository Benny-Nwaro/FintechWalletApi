using FintechWalletApi.Services;
using Microsoft.AspNetCore.Mvc;
using FintechWalletApi.DTOs;
using Microsoft.AspNetCore.RateLimiting;
namespace FintechWalletApi.Controllers;

[ApiController]
[EnableRateLimiting("General")]
[Route("api/transactions")]
public class TransactionsController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionsController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpGet("wallet/{walletId}")]
    public IActionResult GetWalletTransactions(Guid walletId, [FromQuery] TransactionQueryParams query)
    {
        var transactions = _transactionService.GetWalletTransactions(walletId, query);
        return Ok(transactions);
    }
}
