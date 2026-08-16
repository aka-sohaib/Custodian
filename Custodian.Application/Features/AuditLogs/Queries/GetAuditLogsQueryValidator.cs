using FluentValidation;

namespace Custodian.Application.Features.AuditLogs.Queries;

public class GetAuditLogsQueryValidator : AbstractValidator<GetAuditLogsQuery>
{
    public GetAuditLogsQueryValidator()
    {
        //---- Validate PageNumber ----
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be greater than or equal to 1.");

        //---- Validate PageSize ----
        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");

        //---- Validate Date bounds ----
        RuleFor(x => x.ToDate)
            .GreaterThanOrEqualTo(x => x.FromDate!.Value)
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue)
            .WithMessage("To date must be greater than or equal to from date.");
    }
}
