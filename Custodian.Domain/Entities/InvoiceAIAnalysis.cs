namespace Custodian.Domain.Entities
{
    public class InvoiceAIAnalysis : BaseEntity
    {
        //---- For EF Core ----
        private InvoiceAIAnalysis() { }

        //---- For Factory ----
        private InvoiceAIAnalysis(Guid id, Guid invoiceId, Guid categoryId, bool isAnomaly, string? anomalyReason, 
                                  decimal confidenceScore) : base(id)
        {
            InvoiceId       = invoiceId;
            CategoryId      = categoryId;
            IsAnomaly       = isAnomaly;
            AnomalyReason   = anomalyReason;
            ConfidenceScore = confidenceScore;
        }

        //---- Factory Method ----
        public static InvoiceAIAnalysis Create(Guid invoiceId, Guid categoryId, bool isAnomaly, decimal confidenceScore,
                                               string? anomalyReason = null)
        {
            if (invoiceId == Guid.Empty)
                throw new ArgumentException("Invoice ID is required.", nameof(invoiceId));
            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category ID is required.", nameof(categoryId));
            if (confidenceScore < 0 || confidenceScore > 1)
                throw new ArgumentException("Confidence score must be between 0 and 1.", nameof(confidenceScore));
            if (isAnomaly && string.IsNullOrWhiteSpace(anomalyReason))
                throw new ArgumentException("Anomaly reason is required when IsAnomaly is true.", nameof(anomalyReason));

            return new InvoiceAIAnalysis(Guid.NewGuid(), invoiceId, categoryId, isAnomaly, anomalyReason, confidenceScore);
        }

        //---- Properties ----
        public bool IsAnomaly { get; private set; }
        public string? AnomalyReason { get; private set; }
        public decimal ConfidenceScore { get; private set; }

        //---- Foreign Keys ----
        public Guid InvoiceId { get; private set; }
        public Guid CategoryId { get; private set; }

        //---- Navigation Properties ----
        public Invoice Invoice { get; private set; } = null!;
        public Category Category { get; private set; } = null!;
    }
}
