using Custodian.Domain.Entities;
using Custodian.Domain.Enums;
using Custodian.Domain.Interfaces;
using Custodian.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Custodian.Infrastructure.Repositories;
public class AuditLogRepository: IAuditLogRepository
{
    private readonly CustodianDbContext _context;
    public AuditLogRepository(CustodianDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    public async Task<IEnumerable<AuditLog>> GetByTargetAsync(AuditTargetType targetType, Guid targetId)
    {
        return await _context.AuditLogs.Where(a => (a.TargetType == targetType && a.TargetId == targetId)).ToListAsync();
    }
    public async Task<IEnumerable<AuditLog>> GetByUserAsync(Guid userId)
    {
        return await _context.AuditLogs.Where(a => a.PerformedById == userId).ToListAsync();
    }

    public async Task<(IEnumerable<AuditLog> Items, int TotalCount)> GetFilteredAuditLogsAsync(
        Guid orgId,
        AuditTargetType? targetType = null,
        Guid? targetId = null,
        AuditAction? action = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int pageNumber = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        //---- Start with read-only query including PerformedBy user ----
        var query = _context.AuditLogs
            .AsNoTracking()
            .Include(a => a.PerformedBy)
            .Where(a => a.PerformedBy.OrganizationId == orgId);

        //---- Apply optional filters ----
        if (targetType.HasValue)
        {
            query = query.Where(a => a.TargetType == targetType.Value);
        }

        if (targetId.HasValue)
        {
            query = query.Where(a => a.TargetId == targetId.Value);
        }

        if (action.HasValue)
        {
            query = query.Where(a => a.Action == action.Value);
        }

        if (fromDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(a => a.CreatedAt <= toDate.Value);
        }

        //---- Count matching audit logs before pagination ----
        int totalCount = await query.CountAsync(cancellationToken);

        //---- Apply sorting and pagination ----
        int validPageSize = pageSize > 0 ? pageSize : 10;
        int validPageNumber = pageNumber > 0 ? pageNumber : 1;

        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((validPageNumber - 1) * validPageSize)
            .Take(validPageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task AddAsync(AuditLog auditLog)
    {
        await _context.AuditLogs.AddAsync(auditLog);
        await _context.SaveChangesAsync();
    }
}
