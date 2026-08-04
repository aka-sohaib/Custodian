using Custodian.Domain.Enums;
using Custodian.Domain.Interfaces;

namespace Custodian.Domain.Entities
{
    public class Invoice : BaseEntity, IAuditable
    {
        //---- For EF Core ----
        private Invoice() { }

        //---- For Factory ----
        private Invoice(Guid id, string invoiceNumber, Guid companyVendorId, Guid submittedById, string currencyCode, DateTime dueDate) : base(id)
        {
            InvoiceNumber   = invoiceNumber;
            CompanyVendorId = companyVendorId;
            SubmittedById   = submittedById;
            CurrencyCode    = currencyCode;
            DueDate         = dueDate;
            TotalAmount     = decimal.Zero;
            CurrentStatus   = Status.Draft;
        }

        //---- Factory Method ----
        public static Invoice Create(string invoiceNumber, Guid companyVendorId, Guid submittedById, string currencyCode, DateTime dueDate)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber))
                throw new ArgumentException("Invoice number is required.", nameof(invoiceNumber));
            if (companyVendorId == Guid.Empty)
                throw new ArgumentException("Company Vendor Connection ID is required.", nameof(companyVendorId));
            if (submittedById == Guid.Empty)
                throw new ArgumentException("Submitted by ID is required.", nameof(submittedById));
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code is required.", nameof(currencyCode));
            if (dueDate <= DateTime.UtcNow)
                throw new ArgumentException("Due date must be in the future.", nameof(dueDate));

            return new Invoice(Guid.NewGuid(), invoiceNumber, companyVendorId, submittedById, currencyCode, dueDate);
        }

        //---- Update Method ----
        public void Update(string currencyCode, DateTime dueDate)
        {
            if (this.CurrentStatus != Status.Draft)
                throw new InvalidOperationException("You can only edit an invoice while it is in Draft status.");
            if (string.IsNullOrWhiteSpace(currencyCode))
                throw new ArgumentException("Currency code is required.", nameof(currencyCode));
            if (dueDate <= DateTime.UtcNow)
                throw new ArgumentException("Due date must be in the future.", nameof(dueDate));

            CurrencyCode = currencyCode;
            DueDate      = dueDate;
            UpdatedAt    = DateTime.UtcNow;
        }

        //---- Status Update Methods ----
        public void Submit()
        {
            if (CurrentStatus != Status.Draft)
                throw new InvalidOperationException($"Cannot submit invoice from state: {CurrentStatus}");

            CurrentStatus = Status.Submitted;
            UpdatedAt     = DateTime.UtcNow;
        }
        public void BeginReview()
        {
            if (CurrentStatus != Status.Submitted)
                throw new InvalidOperationException($"Cannot begin review from state: {CurrentStatus}");

            CurrentStatus = Status.UnderReview;
            UpdatedAt     = DateTime.UtcNow;
        }
        public void Approve()
        {
            if (CurrentStatus != Status.UnderReview)
                throw new InvalidOperationException($"Cannot approve from state: {CurrentStatus}");

            CurrentStatus = Status.Approved;
            UpdatedAt     = DateTime.UtcNow;
        }
        public void Reject(string reason)
        {
            if (CurrentStatus != Status.UnderReview && CurrentStatus != Status.Submitted)
                throw new InvalidOperationException("Invoice is not in a state that can be rejected.");
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("You must provide a reason for rejection.");
            CurrentStatus = Status.Rejected;
            UpdatedAt     = DateTime.UtcNow;
        }

        //---- Properties ----
        public string InvoiceNumber { get; init; } = null!;
        public decimal TotalAmount { get; private set; } = decimal.Zero;
        public Status CurrentStatus { get; private set; } = Status.Draft;
        public DateTime DueDate { get; private set; }
        public string CurrencyCode { get; private set; } = null!;

        //---- Foreign Keys ----
        public Guid CompanyVendorId { get; private set; }
        public Guid SubmittedById { get; private set; }

        //---- Navigation Properties ----
        public CompanyVendor CompanyAndVendor { get; private set; } = null!;
        public User SubmittedBy { get; private set; } = null!;
        public ICollection<LineItem> LineItems { get; private set; } = new List<LineItem>();
        public InvoiceAIAnalysis? AIAnalysis { get; private set; }
    }
}
