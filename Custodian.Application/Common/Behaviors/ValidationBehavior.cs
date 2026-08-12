using FluentValidation;
using MediatR;

namespace Custodian.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        //---- If no validators, return ----
        if (!_validators.Any())
        {
            return await next();
        }

        //---- wrap the context for validator ----
        var context = new ValidationContext<TRequest>(request);

        //---- Run all the validators for this command ----
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        //---- Wrap all the errors into list ----
        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        //---- Throw exception if errors ----
        if(failures.Count() != 0)
        {
            throw new ValidationException(failures);
        }

        return await next();
    }
}
