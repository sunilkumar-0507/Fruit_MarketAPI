using System.Net;
using FluentValidation;
using Fruitmarket.Application.Common;

namespace Fruitmarket.API.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await WriteAsync(context, StatusCodes.Status400BadRequest, "Validation failed", ex.Errors.Select(e => e.ErrorMessage));
        }
        catch (ApiException ex)
        {
            await WriteAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled API exception");
            await WriteAsync(context, (int)HttpStatusCode.InternalServerError, "An unexpected error occurred");
        }
    }

    private static async Task WriteAsync(HttpContext context, int statusCode, string message, IEnumerable<string>? errors = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new { statusCode, message, errors });
    }
}
