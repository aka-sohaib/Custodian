namespace Custodian.Application.DTOs;
public record ExtractedInvoiceDto(
    string? VendorName,
    string? InvoiceNumber,
    DateTime? Date,
    DateTime? DueDate,
    string? CurrencyCode,
    decimal? TotalAmount,
    List<ExtractedLineItemDto> LineItems
);
