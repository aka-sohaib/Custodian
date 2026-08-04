namespace Custodian.Domain.Entities
{
    public class LineItem : BaseEntity
    {
        //---- For EF Core ----
        private LineItem() { }

        //---- For Factory ----
        private LineItem(Guid id, Guid invoiceId, string description, decimal quantity, decimal unitPrice) : base(id)
        {
            InvoiceId   = invoiceId;
            Description = description;
            Quantity    = quantity;
            UnitPrice   = unitPrice;
        }

        //---- Factory Method ----
        public static LineItem Create(Guid invoiceId, string description, decimal quantity, decimal unitPrice)
        {
            if (invoiceId == Guid.Empty)
                throw new ArgumentException("Invoice ID is required.", nameof(invoiceId));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.", nameof(description));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            if (unitPrice <= 0)
                throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));

            return new LineItem(Guid.NewGuid(), invoiceId, description, quantity, unitPrice);
        }

        //---- Update Method ----
        public void UpdateLineItem(string description, decimal quantity, decimal unitPrice)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description is required.", nameof(description));
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.", nameof(quantity));
            if (unitPrice <= 0)
                throw new ArgumentException("Unit price must be greater than zero.", nameof(unitPrice));

            Description = description;
            Quantity    = quantity;
            UnitPrice   = unitPrice;
            UpdatedAt   = DateTime.UtcNow;
        }

        //---- Properties ----
        public string Description { get; private set; } = null!;
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice => UnitPrice * Quantity;

        //---- Foreign Keys ----
        public Guid InvoiceId { get; private set; }

        //---- Navigation Properties ----
        public Invoice Invoice { get; private set; } = null!;
    }
}
