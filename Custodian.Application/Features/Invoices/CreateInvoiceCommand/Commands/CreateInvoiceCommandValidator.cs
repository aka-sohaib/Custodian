using FluentValidation;
namespace Custodian.Application.Features.Invoices.CreateInvoiceCommand.Commands;

public class CreateInvoiceCommandValidator: AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.InvoiceNumber)
            .NotEmpty().WithMessage("Invoice Number is required.");

        RuleFor(x => x.CurrencyCode)
            .NotEmpty().Length(3).WithMessage("Currency code must be 3-letter ISO Code");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow);

        RuleFor(x => x.LineItems).NotEmpty().WithMessage("At least one line item is required.");
        RuleForEach(x => x.LineItems).SetValidator(new CreateLineItemDtoValidator());

        RuleFor(x => x)
            .Must(HaveExactlyOneVendorIdentity)
            .WithMessage("You must provide either an Organization Connection ID OR an Unregistered Vendor Name, but not both.");
    }

    private bool HaveExactlyOneVendorIdentity(CreateInvoiceCommand command)
    {
        bool hasOrgId = command.OrganizationConnectionId.HasValue && command.OrganizationConnectionId.Value != Guid.Empty;
        bool hasVendorName = !string.IsNullOrWhiteSpace(command.UnregisteredVendorName);

        return hasOrgId ^ hasVendorName;
    }
}


