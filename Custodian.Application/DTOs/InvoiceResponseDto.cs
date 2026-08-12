namespace Custodian.Application.DTOs;

public record InvoiceResponseDto(
    Guid Id,
    string InvoiceNumber,
    string CurrencyCode,
    string? VendorName,
    DateTime DueDate,
    decimal TotalAmount,
    string Status,
    List<LineItemDto> LineItems
);
