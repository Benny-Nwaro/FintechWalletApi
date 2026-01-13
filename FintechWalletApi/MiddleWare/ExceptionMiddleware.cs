using System.Net;
using System.Text.Json;
using FintechWalletApi.Errors;
using FintechWalletApi.Exceptions;

namespace FintechWalletApi.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            await HandleException(context, ex.StatusCode, ex.Message);
        }
        catch (Exception)
        {
            await HandleException(
                context,
                (int)HttpStatusCode.InternalServerError,
                "An unexpected error occurred"
            );
        }
    }

    private static async Task HandleException(
        HttpContext context,
        int statusCode,
        string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
