namespace Custodian.Application.DTOs;

public record AuditLogResponseDto(
    Guid Id,
    string Action,
    string TargetType,
    Guid TargetId,
    Guid PerformedById,
    string PerformedByName,
    string PerformedByEmail,
    DateTime CreatedAt
);
