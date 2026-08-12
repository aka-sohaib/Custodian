using Custodian.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Custodian.Api.ExceptionHandlers
{
    public class ExternalServiceExceptionHandler: IExceptionHandler
    {
        private readonly ILogger<ExternalServiceExceptionHandler> _logger;
        public ExternalServiceExceptionHandler(ILogger<ExternalServiceExceptionHandler> logger) { _logger = logger; }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) 
        { 
            //---- Check if external service exceptio ----
            if(exception is not ExternalServiceException externalServiceException)
            {
                return false;
            }

            //---- Log the raw message first ----
            _logger.LogError(exception, "External service failed: {Message}", exception.Message);

            //---- build a RFC response ----
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status502BadGateway,
                Title = "External Service Failure",
                Detail = "The service could not process the request at this time. Please try again later."
            };

            //---- write to the http response ----
            httpContext.Response.StatusCode = problemDetails.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
