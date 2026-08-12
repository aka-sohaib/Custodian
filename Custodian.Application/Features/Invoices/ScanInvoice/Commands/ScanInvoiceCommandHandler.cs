using Custodian.Application.Common.Interfaces;
using Custodian.Application.DTOs;
using MediatR;

namespace Custodian.Application.Features.Invoices.ScanInvoice.Commands;

public class ScanInvoiceCommandHandler : IRequestHandler<ScanInvoiceCommand, ExtractedInvoiceDto>
{
    private readonly IInvoiceScanner _invoiceScanner;
    public ScanInvoiceCommandHandler(IInvoiceScanner invoiceScanner) { _invoiceScanner = invoiceScanner; }

    public async Task<ExtractedInvoiceDto> Handle(ScanInvoiceCommand command, CancellationToken cancellationToken)
    {
        //---- Reading the raw binary data ----
        using var fileStream = command.File.OpenReadStream();

        //---- scan & extract invoice ----
        var extractedData = await _invoiceScanner.ScanAsync(fileStream, cancellationToken);
        
        return extractedData;
    }
}
