namespace Custodian.Application.DTOs;

public record ExtractedLineItemDto(
    string? Description,
    decimal? Quantity,
    decimal? UnitPrice,
    decimal? ExtractedTotalPrice
);