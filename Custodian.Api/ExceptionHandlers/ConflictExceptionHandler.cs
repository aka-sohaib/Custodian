using Custodian.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Custodian.Api.ExceptionHandlers
{
    public class ConflictExceptionHandler: IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            //---- check if conflict exception ----
            if(exception is not ConflictException conflictEx)
            {
                return false;
            }

            //---- build the RFC 7807 response ----
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Resource Conflict",
                Detail = conflictEx.Message
            };

            //---- Write the error to response ----
            context.Response.StatusCode = problemDetails.Status.Value;
            await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
