using Custodian.Application.Common.Exceptions;
using Custodian.Application.Common.Interfaces;
using Custodian.Application.Common.Models;
using Custodian.Application.DTOs;
using Custodian.Domain.Interfaces;
using MediatR;

namespace Custodian.Application.Features.AuditLogs.Queries;

public class GetAuditLogsQueryHandler : IRequestHandler<GetAuditLogsQuery, PagedList<AuditLogResponseDto>>
{
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IVendorUserRepository _vendorUserRepository;
    private readonly IInternalUserRepository _internalUserRepository;
    private readonly ICurrentUserService _currentUserService;

    public GetAuditLogsQueryHandler(
        IAuditLogRepository auditLogRepository,
        IVendorUserRepository vendorUserRepository,
        IInternalUserRepository internalUserRepository,
        ICurrentUserService currentUserService)
    {
        _auditLogRepository     = auditLogRepository;
        _vendorUserRepository   = vendorUserRepository;
        _internalUserRepository = internalUserRepository;
        _currentUserService     = currentUserService;
    }

    public async Task<PagedList<AuditLogResponseDto>> Handle(GetAuditLogsQuery query, CancellationToken cancellationToken)
    {
        //---- Extract user ID from JWT bearer context ----
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
        Guid orgId = isVendor ? vendorUser!.OrganizationId : internalUser!.OrganizationId;

        //---- Check role authorization (Viewer users cannot view audit logs) ----
        if (!isVendor && internalUser!.InternalUserRole == Domain.Enums.InternalUserRole.Viewer)
        {
            throw new UnauthorizedException("Viewer role users are not authorized to view organization audit logs.");
        }

        //---- Fetch filtered audit logs for organization ----
        var (logs, totalCount) = await _auditLogRepository.GetFilteredAuditLogsAsync(
            orgId: orgId,
            targetType: query.TargetType,
            targetId: query.TargetId,
            action: query.Action,
            fromDate: query.FromDate,
            toDate: query.ToDate,
            pageNumber: query.PageNumber,
            pageSize: query.PageSize,
            cancellationToken: cancellationToken
        );

        //---- Map to AuditLogResponseDto ----
        var dtos = logs.Select(a => new AuditLogResponseDto(
            a.Id,
            a.Action.ToString(),
            a.TargetType.ToString(),
            a.TargetId,
            a.PerformedById,
            a.PerformedBy != null ? a.PerformedBy.Name : "Unknown User",
            a.PerformedBy?.Email ?? string.Empty,
            a.CreatedAt
        )).ToList();

        //---- Return paginated list container ----
        return PagedList<AuditLogResponseDto>.Create(dtos, totalCount, query.PageNumber, query.PageSize);
    }
}
