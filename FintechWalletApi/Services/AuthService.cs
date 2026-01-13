using FintechWalletApi.Data;
using FintechWalletApi.Models;
using FintechWalletApi.Security;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FintechWalletApi.Audit;
namespace FintechWalletApi.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly IAuditService _auditService;

    public AuthService(AppDbContext context, IConfiguration config, IAuditService auditService)
    {
        _context = context;
        _config = config;
        _auditService = auditService;
    }

    public async Task<string> Register(string email, string password)
    {
        var user = new User
        {
            Email = email,
            PasswordHash = PasswordHasher.Hash(password)
        };

        _context.Users.Add(user);
        _context.SaveChanges();
        await _auditService.LogAsync(
            user.Id,
            "UserRegistered",
            "Auth",
            "Success",
            new { user.Email });


        return GenerateToken(user);
    }

    public async Task<string> Login(string email, string password)
    {
        var hash = PasswordHasher.Hash(password);

        var user = _context.Users
            .FirstOrDefault(u => u.Email == email && u.PasswordHash == hash)
            ?? throw new Exception("Invalid credentials");



        await _auditService.LogAsync(
            user.Id,
            "UserRegistered",
            "Auth",
            "Success",
            new { user.Email });

        return GenerateToken(user);
    }



    private string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Jwt:ExpiryMinutes"]!)),
            signingCredentials: new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
