using FintechWalletApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace FintechWalletApi.Controllers;

[ApiController]
[EnableRateLimiting("Money")]
[Authorize]
[Route("api/wallets")]
public class WalletController : ControllerBase
{
    private readonly IWalletService _walletService;

    public WalletController(IWalletService walletService)
    {
        _walletService = walletService;
    }

    [HttpPost("create")]
    public IActionResult CreateWallet(Guid userId)
    {
        var wallet = _walletService.CreateWallet(userId);
        return Ok(wallet);
    }

    [HttpPost("{walletId}/add")]
    public IActionResult AddMoney(Guid walletId, decimal amount)
    {
        var wallet = _walletService.AddMoney(walletId, amount);
        return Ok(wallet);
    }

    [HttpPost("{walletId}/withdraw")]
    public IActionResult WithdrawMoney(Guid walletId, decimal amount)
    {
        var wallet = _walletService.WithdrawMoney(walletId, amount);
        return Ok(wallet);
    }

    [HttpPost("transfer")]
    public IActionResult Transfer(Guid fromWalletId, Guid toWalletId,decimal amount)
        {
            var wallet = _walletService.Transfer(fromWalletId, toWalletId, amount);
            return Ok(wallet);
        }

}
