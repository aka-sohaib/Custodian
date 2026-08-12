using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Custodian.Api.ExceptionHandlers
{
    public class GlobalEceptionHandler: IExceptionHandler
    {
        private readonly ILogger<GlobalEceptionHandler> _logger;

        public GlobalEceptionHandler(ILogger<GlobalEceptionHandler> logger) => _logger = logger;

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            //---- Log Actual Error to log files ----
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            //---- Build an error message ----
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title  = "Ineternal Server Error",
                Detail = "An unexpected fault happened. Please try again later."
            };

            //---- Write the error details to response ----
            context.Response.StatusCode = problemDetails.Status.Value;
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
