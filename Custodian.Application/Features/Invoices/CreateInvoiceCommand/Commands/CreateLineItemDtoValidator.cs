using Custodian.Application.DTOs;
using FluentValidation;

namespace Custodian.Application.Features.Invoices.CreateInvoiceCommand.Commands;

public class CreateLineItemDtoValidator : AbstractValidator<CreateLineItemDto>
{
    public CreateLineItemDtoValidator()
    {
        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description for the item is required.");
            
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
            
        RuleFor(x => x.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than 0");
    }
}
