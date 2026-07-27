using Custodian.Domain.Entities;
using Custodian.Domain.Enums;

namespace Custodian.Domain.Interfaces
{
    public interface IAuditLogRepository
    {
        Task<IEnumerable<AuditLog>> GetByTargetAsync(AuditTargetType targetType, Guid targetId);
        Task<IEnumerable<AuditLog>> GetByUserAsync(Guid userId);
        Task AddAsync(AuditLog auditLog);
    }
}
