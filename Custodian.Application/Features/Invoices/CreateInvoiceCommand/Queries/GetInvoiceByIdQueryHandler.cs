using Custodian.Application.Common.Exceptions;
using Custodian.Application.DTOs;
using Custodian.Domain.Interfaces;
using MediatR;

namespace Custodian.Application.Features.Invoices.CreateInvoiceCommand.Queries;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceResponseDto?>
{
    private readonly IInvoiceRepository _invoiceRepository;

    public GetInvoiceByIdQueryHandler(IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
    }

    public async Task<InvoiceResponseDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        //---- Fetch the invoice ----
        var invoice = await _invoiceRepository.GetByIdAsync(request.id, readOnly: true);

        //---- Validation ----
        if (invoice == null)
            throw new NotFoundException(nameof(invoice), request.id);

        //---- Map line items ----
        var lineItemDtos = invoice.LineItems.Select(lineItem => new LineItemDto(
            lineItem.Id,
            lineItem.Description,
            lineItem.Quantity,
            lineItem.UnitPrice,
            lineItem.TotalPrice
        )).ToList();

        //---- Determine vendor name ----
        var vendorName = invoice.OrganizationConnection?.SellerOrganization?.Name 
            ?? invoice.UnregisteredVendorName;

        //---- Calculate total amount from line items if invoice total is 0 ----
        var totalAmount = invoice.LineItems.Any() 
            ? invoice.LineItems.Sum(li => li.TotalPrice) 
            : invoice.TotalAmount;

        //---- Return invoice response dto ----
        return new InvoiceResponseDto(
            invoice.Id,
            invoice.InvoiceNumber,
            invoice.CurrencyCode,
            vendorName,
            invoice.DueDate,
            totalAmount,
            invoice.CurrentStatus.ToString(),
            lineItemDtos
        );
    }
}
