namespace Custodian.Application.DTOs;

public record LineItemDto(
    Guid Id,
    string Description,
    decimal Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);
