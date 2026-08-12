using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Custodian.Api.Middleware;

public class ValidationExceptionHandler: IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        //---- Check if it's a Validation exception ----
        if (exception is not ValidationException validationException) 
        {
            return false;
        }

        //---- Transform Erros into key/value pairs ----
        var errors = validationException.Errors
            .GroupBy(e=> e.PropertyName)
            .ToDictionary(g=> g.Key, g=> g.Select(e=> e.ErrorMessage).ToArray());

        //---- Build Error Message ----
        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Failed"
        };

        //---- Write Errors onto repponse ----
        context.Response.StatusCode = problemDetails.Status.Value;
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
