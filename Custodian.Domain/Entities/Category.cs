namespace Custodian.Domain.Entities
{
    public class Category : BaseEntity
    {
        //---- For EF Core ----
        private Category() { }

        //---- For Factory ----
        private Category(Guid id, string name, string code, string? glCode, string? description) : base(id)
        {
            Name        = name;
            Code        = code;
            GlCode      = glCode;
            Description = description;
        }

        //---- Factory Method ----
        public static Category Create(string name, string code, string? glCode = null, string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Category name is required.", nameof(name));
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Category code is required.", nameof(code));

            return new Category(Guid.NewGuid(), name, code, glCode, description);
        }

        //---- Properties ----
        public string Name { get; private set; } = null!;
        public string Code { get; private set; } = null!;
        public string? GlCode { get; private set; }
        public string? Description { get; private set; }

        //---- Navigation Properties ----
        public ICollection<InvoiceAIAnalysis> Analyses { get; private set; } = new List<InvoiceAIAnalysis>();
    }
}
