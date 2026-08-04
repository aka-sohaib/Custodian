using Custodian.Domain.Enums;

namespace Custodian.Domain.Entities;

public class CompanyVendor: BaseEntity
{
    private CompanyVendor() { }

    //---- For Factory ----
    private CompanyVendor(Guid companyId, Guid vendorId, Guid requestedById, Guid? respondedById, int paymentTermDays) : base(Guid.NewGuid())
    {
        CompanyId        = companyId;
        VendorId         = vendorId;
        RequestedById    = requestedById;
        RespondedById    = respondedById;
        ConnectionStatus = ConnectionStatus.Pending;
        PaymentTermDays  = paymentTermDays;
    }

    //---- Factory Method ----
    public static CompanyVendor CreateCompanyVendorConnection(Guid companyId, Guid vendorId, Guid requestedById , int paymentTermDays)
    {
        if (companyId == Guid.Empty)
            throw new ArgumentException("Company Id is required.", nameof(companyId));
        if (vendorId == Guid.Empty)
            throw new ArgumentException("Vendor Id is required.", nameof(vendorId));
        if (requestedById == Guid.Empty)
            throw new ArgumentException("Internal User Id is required.", nameof(requestedById));
        if (paymentTermDays <= 0)
            throw new ArgumentException("Payment Term Days cant be fewer than 1.", nameof(paymentTermDays));
        
        return new CompanyVendor(companyId, vendorId, requestedById, respondedById: null , paymentTermDays);
    }

    //---- Update ----
    public void UpdateCompanyVendorConnection(int paymentTermDays)
    {
        if (paymentTermDays <= 0)
            throw new ArgumentException("Payment Term Days cant be fewer than 1.", nameof(paymentTermDays));
        
        PaymentTermDays = paymentTermDays;
        UpdatedAt       = DateTime.UtcNow;
    }

    //---- Accept Connection ----
    public void AcceptConnection(Guid respondedById)
    {
        if (respondedById == Guid.Empty)
            throw new ArgumentException("Responder ID is required.", nameof(respondedById));

        ConnectionStatus = ConnectionStatus.Active;
        RespondedById = respondedById;
        RespondedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    //---- Reject Connection ----
    public void RejectConnection(Guid respondedById)
    {
        if (respondedById == Guid.Empty)
            throw new ArgumentException("Responder ID is required.", nameof(respondedById));
        ConnectionStatus = ConnectionStatus.Rejected;
        RespondedById = respondedById;
        RespondedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    //---- Properties ----
    public Guid CompanyId                    { get; private set; } = Guid.Empty;
    public Guid VendorId                     { get; private set; } = Guid.Empty;
    public Guid RequestedById                { get; private set; } = Guid.Empty;
    public Guid? RespondedById               { get; private set; }
    public ConnectionStatus ConnectionStatus { get; private set; }
    public int PaymentTermDays               { get; private set; }
    public DateTime? RespondedAt             { get; private set; } = null;

    //---- Navigational Properties ----
    public Vendor Vendor             { get; private set; } = null!;
    public Company Company           { get; private set; } = null!;
    public InternalUser InternalUser { get; private set; } = null!;
    public VendorUser? VendorUser    { get; private set; } = null!;
}
