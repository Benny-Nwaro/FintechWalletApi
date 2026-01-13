using System.Text;
using FintechWalletApi.Idempotency;

namespace FintechWalletApi.Middleware;

public class IdempotencyMiddleware
{
    private const string HeaderName = "Idempotency-Key";
    private readonly RequestDelegate _next;

    public IdempotencyMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IIdempotencyService idempotencyService)
    {
        if (!context.Request.Headers.TryGetValue(HeaderName, out var key))
        {
            await _next(context);
            return;
        }

        context.Request.EnableBuffering();

        var body = await new StreamReader(
            context.Request.Body).ReadToEndAsync();

        context.Request.Body.Position = 0;

        var bodyHash = IdempotencyService.HashBody(body);
        var path = context.Request.Path.ToString();

        var cachedResponse =
            await idempotencyService.GetCachedResponseAsync(
                key!, path, bodyHash);

        if (cachedResponse != null)
        {
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(cachedResponse);
            return;
        }

        // Capture response
        var originalBody = context.Response.Body;
        using var memStream = new MemoryStream();
        context.Response.Body = memStream;

        await _next(context);

        memStream.Position = 0;
        var responseBody = await new StreamReader(memStream).ReadToEndAsync();

        await idempotencyService.SaveResponseAsync(
            key!, path, bodyHash, responseBody);

        memStream.Position = 0;
        await memStream.CopyToAsync(originalBody);
    }
}
