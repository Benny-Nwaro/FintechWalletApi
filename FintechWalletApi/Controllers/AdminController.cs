using FintechWalletApi.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FintechWalletApi.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IAdminReportService _reportService;

    public AdminController(IAdminReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpGet("daily-volume")]
    public IActionResult GetDailyVolume(DateTime date)
    {
        return Ok(_reportService.GetDailyVolume(date));
    }

    [HttpGet("top-wallets")]
    public IActionResult GetTopWallets(int top = 5)
    {
        return Ok(_reportService.GetTopWallets(top));
    }

    [HttpGet("fraud-summary")]
    public IActionResult GetFraudSummary(
        DateTime from,
        DateTime to)
    {
        return Ok(_reportService.GetFraudSummary(from, to));
    }
}
