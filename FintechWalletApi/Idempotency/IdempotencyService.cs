using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using FintechWalletApi.Data;

namespace FintechWalletApi.Idempotency;

public class IdempotencyService : IIdempotencyService
{
    private readonly AppDbContext _context;

    public IdempotencyService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string?> GetCachedResponseAsync(
        string key,
        string path,
        string bodyHash)
    {
        var record = await _context.IdempotencyKeys
            .FirstOrDefaultAsync(x =>
                x.Key == key &&
                x.RequestPath == path &&
                x.RequestBodyHash == bodyHash);

        return record?.Response;
    }

    public async Task SaveResponseAsync(
        string key,
        string path,
        string bodyHash,
        string response)
    {
        _context.IdempotencyKeys.Add(new IdempotencyKey
        {
            Key = key,
            RequestPath = path,
            RequestBodyHash = bodyHash,
            Response = response
        });

        await _context.SaveChangesAsync();
    }

    public static string HashBody(string body)
    {
        using var sha = SHA256.Create();
        return Convert.ToBase64String(
            sha.ComputeHash(Encoding.UTF8.GetBytes(body)));
    }
}
