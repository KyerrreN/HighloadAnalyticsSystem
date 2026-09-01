using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Telemetry.Ingress.API.Infrastructure.Exceptions;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is UserManagementUnavailableException ex)
        {
            logger.LogError(ex, "UserManagement gRPC service is unavailable.");

            httpContext.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status503ServiceUnavailable,
                Title = "Service Unavailable",
                Detail = ex.Message
            }, cancellationToken);

            return true;
        }

        if (exception is BadHttpRequestException or JsonException)
        {
            logger.LogWarning("Malformed JSON or bad request payload: {Message}", exception.Message);

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid Request Payload",
                Detail = "The provided JSON body is malformed or contains syntax errors."
            }, cancellationToken);

            return true;
        }

        return false;
    }
}
