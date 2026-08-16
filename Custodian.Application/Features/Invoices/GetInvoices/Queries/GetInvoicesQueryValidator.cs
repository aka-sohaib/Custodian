using FluentValidation;

namespace Custodian.Application.Features.Invoices.GetInvoices.Queries;

public class GetInvoicesQueryValidator : AbstractValidator<GetInvoicesQuery>
{
    public GetInvoicesQueryValidator()
    {
        //---- Validate PageNumber ----
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1.");

        //---- Validate PageSize ----
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        //---- Validate Amount bounds ----
        RuleFor(x => x.MinAmount)
            .GreaterThanOrEqualTo(0).When(x => x.MinAmount.HasValue)
            .WithMessage("Minimum amount cannot be negative.");

        RuleFor(x => x.MaxAmount)
            .GreaterThanOrEqualTo(x => x.MinAmount!.Value)
            .When(x => x.MinAmount.HasValue && x.MaxAmount.HasValue)
            .WithMessage("Maximum amount must be greater than or equal to minimum amount.");

        //---- Validate Date bounds ----
        RuleFor(x => x.ToDueDate)
            .GreaterThanOrEqualTo(x => x.FromDueDate!.Value)
            .When(x => x.FromDueDate.HasValue && x.ToDueDate.HasValue)
            .WithMessage("To due date must be greater than or equal to from due date.");
    }
}
