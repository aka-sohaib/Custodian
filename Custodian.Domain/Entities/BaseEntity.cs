namespace Custodian.Domain.Entities
{
    public abstract class BaseEntity
    {
        //---- For EF Core ----
        protected BaseEntity() { }

        //---- For Child Classes ----
        protected BaseEntity(Guid id)
        {
            Id        = id;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        //---- Properties ----
        public Guid Id { get; protected set; }
        public bool IsDeleted { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        public void Delete()
        {
            if (IsDeleted) return;
            IsDeleted = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
