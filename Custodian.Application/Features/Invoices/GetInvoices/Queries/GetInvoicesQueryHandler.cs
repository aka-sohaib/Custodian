using Custodian.Application.Common.Exceptions;
using Custodian.Application.Common.Interfaces;
using Custodian.Application.Common.Models;
using Custodian.Application.DTOs;
using Custodian.Domain.Interfaces;
using MediatR;

namespace Custodian.Application.Features.Invoices.GetInvoices.Queries;

public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, PagedList<InvoiceResponseDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IVendorUserRepository _vendorUserRepository;
    private readonly IInternalUserRepository _internalUserRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetInvoicesQueryHandler(
        IInvoiceRepository invoiceRepository,
        IVendorUserRepository vendorUserRepository,
        IInternalUserRepository internalUserRepository,
        ICurrentUserService currentUserService)
    {
        _invoiceRepository      = invoiceRepository;
        _vendorUserRepository   = vendorUserRepository;
        _internalUserRepository = internalUserRepository;
        _currentUserService     = currentUserService;
    }

    public async Task<PagedList<InvoiceResponseDto>> Handle(GetInvoicesQuery query, CancellationToken cancellationToken)
    {
        //---- Extract user ID from JWT context ----
        var userId = _currentUserService.UserId;
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedException("User is not authenticated.");
        }

        //---- Resolve user type and OrganizationId ----
        var vendorUser   = await _vendorUserRepository.GetByIdAsync(userId);
        var internalUser = vendorUser == null ? await _internalUserRepository.GetByIdAsync(userId) : null;

        if (vendorUser == null && internalUser == null)
        {
            throw new NotFoundException($"User with ID '{userId}' was not found.");
        }

        bool isVendor = vendorUser != null;
        Guid? userOrgId = isVendor ? vendorUser!.OrganizationId : internalUser!.OrganizationId;

        //---- Fetch filtered invoices from repository ----
        var (invoices, totalCount) = await _invoiceRepository.GetFilteredInvoicesAsync(
            userId: userId,
            userOrgId: userOrgId,
            isVendor: isVendor,
            status: query.Status,
            fromDueDate: query.FromDueDate,
            toDueDate: query.ToDueDate,
            minAmount: query.MinAmount,
            maxAmount: query.MaxAmount,
            searchTerm: query.SearchTerm,
            organizationConnectionId: query.OrganizationConnectionId,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize,
            sortBy: query.SortBy,
            isDescending: query.IsDescending,
            cancellationToken: cancellationToken
        );

        //---- Map domain entities to InvoiceResponseDto ----
        var dtos = invoices.Select(i => new InvoiceResponseDto(
            i.Id,
            i.InvoiceNumber,
            i.CurrencyCode,
            i.UnregisteredVendorName ?? i.OrganizationConnection?.SellerOrganization?.Name ?? "Registered Vendor",
            i.DueDate,
            i.TotalAmount,
            i.CurrentStatus.ToString(),
            i.LineItems.Select(l => new LineItemDto(l.Id, l.Description, l.Quantity, l.UnitPrice, l.TotalPrice)).ToList()
        )).ToList();

        //---- Return paginated list container ----
        return PagedList<InvoiceResponseDto>.Create(dtos, totalCount, query.PageNumber, query.PageSize);
    }
}
