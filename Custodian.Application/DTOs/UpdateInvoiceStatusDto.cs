using Custodian.Domain.Enums;

namespace Custodian.Application.DTOs;

public record UpdateInvoiceStatusDto(
    Status NewStatus,
    string? RejectionReason = null
);
