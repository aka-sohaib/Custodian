using Custodian.Application.Common.Models;
using Custodian.Application.DTOs;
using Custodian.Domain.Enums;
using MediatR;

namespace Custodian.Application.Features.AuditLogs.Queries;

public record GetAuditLogsQuery(
    AuditTargetType? TargetType = null,
    Guid? TargetId = null,
    AuditAction? Action = null,
    DateTime? FromDate = null,
    DateTime? ToDate = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PagedList<AuditLogResponseDto>>;
