using Custodian.Application.DTOs;
using MediatR;

namespace Custodian.Application.Features.Invoices.CreateInvoiceCommand.Commands;

public record CreateInvoiceCommand(
        string InvoiceNumber,
        string CurrencyCode,
        string? UnregisteredVendorName,
        Guid? OrganizationConnectionId,
        DateTime DueDate,
        List<CreateLineItemDto> LineItems
    ) : IRequest<Guid>;