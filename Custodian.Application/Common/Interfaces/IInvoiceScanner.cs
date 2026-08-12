using Custodian.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace Custodian.Application.Common.Interfaces;

public interface IInvoiceScanner
{
    Task<ExtractedInvoiceDto> ScanAsync(Stream fileStream, CancellationToken cancellationToken);
}
