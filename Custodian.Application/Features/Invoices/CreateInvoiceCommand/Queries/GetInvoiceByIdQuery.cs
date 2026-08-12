using Custodian.Application.DTOs;
using MediatR;

namespace Custodian.Application.Features.Invoices.CreateInvoiceCommand.Queries;

public record GetInvoiceByIdQuery(Guid id) : IRequest<InvoiceResponseDto>;