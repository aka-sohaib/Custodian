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
    public async Task AddAsync(AuditLog auditLog)
    {
        await _context.AuditLogs.AddAsync(auditLog);
        await _context.SaveChangesAsync();
    }
}
