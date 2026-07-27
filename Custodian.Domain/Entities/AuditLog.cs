using Custodian.Domain.Enums;

namespace Custodian.Domain.Entities
{
    public class AuditLog : BaseEntity
    {
        //---- For EF Core ----
        private AuditLog() { }

        //---- For Factory ----
        private AuditLog(Guid id, AuditAction action, AuditTargetType targetType, Guid targetId, Guid performedById) : base(id)
        {
            Action        = action;
            TargetType    = targetType;
            TargetId      = targetId;
            PerformedById = performedById;
        }

        //---- Factory Method ----
        public static AuditLog Create(AuditAction action, AuditTargetType targetType, Guid targetId, Guid performedById)
        {
            if (targetId == Guid.Empty)
                throw new ArgumentException("Target ID is required.", nameof(targetId));
            if (performedById == Guid.Empty)
                throw new ArgumentException("Performed by ID is required.", nameof(performedById));

            return new AuditLog(Guid.NewGuid(), action, targetType, targetId, performedById);
        }

        //---- Properties ----
        public AuditAction Action { get; private set; }

        //---- Generic Refrences ----
        public AuditTargetType TargetType { get; private set; }
        public Guid TargetId { get; private set; }

        //---- Foreign Keys ----
        public Guid PerformedById { get; private set; }

        //---- Navigation Properties ----
        public User PerformedBy { get; private set; } = null!;
    }
}
