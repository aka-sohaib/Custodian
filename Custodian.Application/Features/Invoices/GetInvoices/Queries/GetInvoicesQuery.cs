using Custodian.Application.Common.Models;
using Custodian.Application.DTOs;
using Custodian.Domain.Enums;
using MediatR;

namespace Custodian.Application.Features.Invoices.GetInvoices.Queries;

public record GetInvoicesQuery(
    Status? Status = null,
    DateTime? FromDueDate = null,
    DateTime? ToDueDate = null,
    decimal? MinAmount = null,
    decimal? MaxAmount = null,
    string? SearchTerm = null,
    Guid? OrganizationConnectionId = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = "CreatedAt",
    bool IsDescending = true
) : IRequest<PagedList<InvoiceResponseDto>>;
