using Custodian.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Custodian.Api.ExceptionHandlers
{
    public class UnauthorizedExceptionHandler: IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken) 
        {
            //---- Check if Authorization Exception ----
            if (exception is not UnauthorizedException && exception is not UnauthorizedAccessException) 
            {
                return false;
            }

            //---- Build the RFC-7807 Response ----
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status401Unauthorized,
                Title  = exception.Message,
            };

            //---- Write Error to Response ----
            context.Response.StatusCode = problemDetails.Status.Value;
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
