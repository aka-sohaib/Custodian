using Custodian.Domain.Enums;
using FluentValidation;

namespace Custodian.Application.Features.Invoices.UpdateInvoiceStatus.Commands;

public class UpdateInvoiceStatusCommandValidator : AbstractValidator<UpdateInvoiceStatusCommand>
{
    public UpdateInvoiceStatusCommandValidator()
    {
        //---- Validate InvoiceId ----
        RuleFor(v => v.InvoiceId)
            .NotEmpty().WithMessage("Invoice ID is required.");

        //---- Validate NewStatus enum ----
        RuleFor(v => v.NewStatus)
            .NotEmpty().WithMessage("Status is required.")
            .IsInEnum().WithMessage("A valid invoice status must be selected.");

        //---- Validate RejectionReason when status is Rejected ----
        RuleFor(v => v.RejectionReason)
            .NotEmpty().WithMessage("Rejection reason is required when rejecting an invoice.")
            .When(v => v.NewStatus == Status.Rejected);
    }
}
