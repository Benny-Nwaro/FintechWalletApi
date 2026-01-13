using FintechWalletApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FintechWalletApi.Controllers;

[ApiController]
[EnableRateLimiting("Auth")]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public IActionResult Register(string email, string password)
    {
        var token = _authService.Register(email, password);
        return Ok(new { token });
    }

    [HttpPost("login")]
    public IActionResult Login(string email, string password)
    {
        var token = _authService.Login(email, password);
        return Ok(new { token });
    }
}
