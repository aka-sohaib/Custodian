using Custodian.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Custodian.Application.Features.Invoices.ScanInvoice.Commands;

public record ScanInvoiceCommand(IFormFile File) : IRequest<ExtractedInvoiceDto>;