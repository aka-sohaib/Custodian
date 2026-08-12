namespace Custodian.Application.DTOs;

public record CreateLineItemDto(
    string Description,
    decimal Quantity,
    decimal UnitPrice
);