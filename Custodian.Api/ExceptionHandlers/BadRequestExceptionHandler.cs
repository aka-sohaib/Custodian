using Custodian.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Custodian.Api.ExceptionHandlers;

public class BadRequestExceptionHandler : IExceptionHandler
{
    private readonly ILogger<BadRequestExceptionHandler> _logger;

    public BadRequestExceptionHandler(ILogger<BadRequestExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        //---- Check if exception is BadRequestException, InvalidOperationException, or ArgumentException ----
        if (exception is not BadRequestException &&
            exception is not InvalidOperationException &&
            exception is not ArgumentException)
        {
            return false;
        }

        //---- Log warning message ----
        _logger.LogWarning(exception, "Bad request / invalid operation: {Message}", exception.Message);

        //---- Build RFC 7807 ProblemDetails response ----
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title  = "Bad Request",
            Detail = exception.Message
        };

        //---- Write response JSON ----
        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
