using Custodian.Domain.Entities;
using Custodian.Domain.Enums;

namespace Custodian.Domain.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<IEnumerable<AuditLog>> GetByTargetAsync(AuditTargetType targetType, Guid targetId);
        Task<IEnumerable<AuditLog>> GetByUserAsync(Guid userId);
        Task<(IEnumerable<AuditLog> Items, int TotalCount)> GetFilteredAuditLogsAsync(
            Guid orgId,
            AuditTargetType? targetType = null,
            Guid? targetId = null,
            AuditAction? action = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default);
        Task AddAsync(AuditLog auditLog);
    }
}
