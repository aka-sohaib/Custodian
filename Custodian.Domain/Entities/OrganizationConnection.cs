using Custodian.Domain.Enums;

namespace Custodian.Domain.Entities;

public class OrganizationConnection : BaseEntity
{
    private OrganizationConnection() { }

    //---- For Factory ----
    private OrganizationConnection(Guid buyerOrganizationId, Guid sellerOrganizationId, Guid requestedById, Guid? respondedById, int paymentTermDays) 
        : base(Guid.NewGuid())
    {
        BuyerOrganizationId  = buyerOrganizationId;
        SellerOrganizationId = sellerOrganizationId;
        RequestedById        = requestedById;
        RespondedById        = respondedById;
        ConnectionStatus     = ConnectionStatus.Pending;
        PaymentTermDays      = paymentTermDays;
    }

    //---- Factory Method ----
    public static OrganizationConnection CreateConnection(Guid buyerOrganizationId, Guid sellerOrganizationId, Guid requestedById, int paymentTermDays)
    {
        if (buyerOrganizationId == Guid.Empty)
            throw new ArgumentException("Buyer Organization ID is required.", nameof(buyerOrganizationId));
        if (sellerOrganizationId == Guid.Empty)
            throw new ArgumentException("Seller Organization ID is required.", nameof(sellerOrganizationId));
        if (buyerOrganizationId == sellerOrganizationId)
            throw new ArgumentException("An organization cannot connect to itself.");
        if (requestedById == Guid.Empty)
            throw new ArgumentException("Requested By User ID is required.", nameof(requestedById));
        if (paymentTermDays <= 0)
            throw new ArgumentException("Payment Term Days cannot be fewer than 1.", nameof(paymentTermDays));

        return new OrganizationConnection(buyerOrganizationId, sellerOrganizationId, requestedById, respondedById: null, paymentTermDays);
    }

    //---- Update Method ----
    public void UpdateConnection(int paymentTermDays)
    {
        if (paymentTermDays <= 0)
            throw new ArgumentException("Payment Term Days cannot be fewer than 1.", nameof(paymentTermDays));

        PaymentTermDays = paymentTermDays;
        UpdatedAt       = DateTime.UtcNow;
    }

    //---- Accept Connection ----
    public void AcceptConnection(Guid respondedById)
    {
        if (respondedById == Guid.Empty)
            throw new ArgumentException("Responder ID is required.", nameof(respondedById));

        ConnectionStatus = ConnectionStatus.Active;
        RespondedById    = respondedById;
        RespondedAt      = DateTime.UtcNow;
        UpdatedAt        = DateTime.UtcNow;
    }

    //---- Reject Connection ----
    public void RejectConnection(Guid respondedById)
    {
        if (respondedById == Guid.Empty)
            throw new ArgumentException("Responder ID is required.", nameof(respondedById));

        ConnectionStatus = ConnectionStatus.Rejected;
        RespondedById    = respondedById;
        RespondedAt      = DateTime.UtcNow;
        UpdatedAt        = DateTime.UtcNow;
    }

    //---- Properties ----
    public Guid BuyerOrganizationId  { get; private set; } = Guid.Empty;
    public Guid SellerOrganizationId { get; private set; } = Guid.Empty;
    public Guid RequestedById        { get; private set; } = Guid.Empty;
    public Guid? RespondedById       { get; private set; }
    public ConnectionStatus ConnectionStatus { get; private set; }
    public int PaymentTermDays       { get; private set; }
    public DateTime? RespondedAt     { get; private set; }

    //---- Navigation Properties ----
    public Organization BuyerOrganization  { get; private set; } = null!;
    public Organization SellerOrganization { get; private set; } = null!;
    public User RequestedBy                { get; private set; } = null!;
    public User? RespondedBy               { get; private set; }
}
