using Custodian.Domain.Enums;
using MediatR;

namespace Custodian.Application.Features.Invoices.UpdateInvoiceStatus.Commands;

public record UpdateInvoiceStatusCommand(
    Guid InvoiceId,
    Status NewStatus,
    string? RejectionReason = null
) : IRequest<bool>;
