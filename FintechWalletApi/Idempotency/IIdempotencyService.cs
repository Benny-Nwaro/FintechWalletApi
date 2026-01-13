namespace FintechWalletApi.Idempotency;

public interface IIdempotencyService
{
    Task<string?> GetCachedResponseAsync(
        string key,
        string path,
        string bodyHash);

    Task SaveResponseAsync(
        string key,
        string path,
        string bodyHash,
        string response);
}
